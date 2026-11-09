#!/usr/bin/env python3
"""Extract Majora's Mask message text into ignored local JSON lookups (stdlib only).

Usage (always call the file, never stdin):
    python -I tools/reference/extract_messages.py                  write both JSON files, print counts
    python -I tools/reference/extract_messages.py --id 180B        print one entry
    python -I tools/reference/extract_messages.py --grep REGEX     matching ids and the first 60 chars
    python -I tools/reference/extract_messages.py --stats          per-box-type and control-code counts

Output (gitignored, Nintendo text, never commit): local-reference/text/messages_nes.json and
messages_staff.json, each {"<4-digit hex id>": entry} in message-table order. An entry carries textId,
textboxType, textboxYPos, the macro's HEADER fields for the main game text (textBoxProperties, itemId,
nextTextId, firstChoicePrice, secondChoicePrice, unk12074), raw, text and lineCount. Ids are hex strings
(nextTextId FFFF means none); every other number is an int. lineCount is the number of newline-separated
lines in text (0 when empty), counted across box breaks. textboxType/textboxYPos are the DEFINE_MESSAGE
table values (always 0 for the main text); the game reads the real box settings from textBoxProperties, so
main-text entries also carry propsHasChoices, propsTextboxType and propsTextboxYPos, its bits 15-12, 11-8
and 7-4 as z_message.c splits them.

Where the text comes from. The pinned zeldaret/mm tree ships assets/text/message_data.h and
message_data_staff.h only as one-line includes of text/message_data*.h, which `make` generates from the ROM
(tools/extract_text.py). That generated text is not in the tree, so this tool decodes the same bytes the
decomp's message tables describe: the message tables inside the extracted `code` segment, the
message_data_static and staff_message_data_static segments, and the control codes, argument widths and names
taken from include/message_data_fmt_nes.h / message_data_fmt_staff.h plus assets/text/charmap.txt.
`raw` is the decoded message written in the decomp's own macro dialect (HEADER(...), "text", COLOR_RED,
SHIFT(n), ...). Whatever macro source is present (always the two end-of-table messages in the stub files,
everything if extracted/<version>/text/message_data*.h exists) is parsed too and must agree with the decode.

Validation, exit code 1 on any failure: every id unique, every control byte and macro recognized (the failure
names it), macro argument counts match the fmt headers, source text matches the decode. Exit 2 = usage/missing
input.
"""
import argparse
import ast
import csv
import json
import re
import struct
import sys
from collections import Counter
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
REF = ROOT / "local-reference"
DECOMP = REF / "mm-decomp"
VERSION = "n64-us"
OUT_DIR = REF / "text"
SEGMENTS_DIR = DECOMP / "extracted" / VERSION / "baserom"
INCLUDE_DIRS = [DECOMP / "extracted" / VERSION]  # the decomp Makefile preprocesses with -Iextracted/<version>

SOURCES = {
    "nes": dict(out="messages_nes.json", stub="assets/text/message_data.h", fmt="include/message_data_fmt_nes.h",
                table="sMessageTableNES", seg="message_data_static", charmap_index=0),
    "staff": dict(out="messages_staff.json", stub="assets/text/message_data_staff.h",
                  fmt="include/message_data_fmt_staff.h", table="sMessageTableCredits",
                  seg="staff_message_data_static", charmap_index=2),
}

END_OF_TABLE_ID = 0xFFFF


class ToolError(Exception):
    pass


class MissingInput(ToolError):
    pass


class Undecodable(Exception):
    pass


# ---------------------------------------------------------------------------------------------------------
# Format headers: control macros, their byte codes and argument layouts, straight from the fmt header text.

ARG_KINDS = {"ARG1": 1, "ARG2": 1, "ARGC": 1, "ARGB1": 1, "ARGB2": 4}  # kind -> number of macro parameters
DEFINE_RE = re.compile(r"^#\s*define\s+([A-Za-z_]\w*)(?:\(([^)]*)\))?(?:\s+(.*))?$")
ARG_RE = re.compile(r"\b(ARG1|ARG2|ARGC|ARGB1|ARGB2)\(([^()]*)\)")
CTRL_RE = re.compile(r"^CTRL_BASE\((\w+)\)\s*(.*)$")
HELPER_MACROS = {"ARG1", "ARG2", "ARGC", "ARGB1", "ARGB2", "CTRL_BASE", "MSG"}
COMMENT_OR_STRING = re.compile(r'//[^\n]*|/\*.*?\*/|"(?:\\.|[^\\"\n])*"', re.S)


def strip_comments(text):
    return COMMENT_OR_STRING.sub(lambda m: " " if m.group(0)[0] == "/" else m.group(0), text)


class Macro:
    def __init__(self, name, params, code, args):
        self.name = name
        self.params = params  # parameter names, in order
        self.code = code  # control byte, None for HEADER
        self.args = args  # [(kind, [parameter names consumed])]


def parse_macro_args(name, params, body):
    args, used = [], 0
    for m in ARG_RE.finditer(body):
        kind = m.group(1)
        n = ARG_KINDS[kind]
        args.append((kind, params[used:used + n]))
        used += n
    if used != len(params):
        raise ToolError(f"fmt header macro {name}: {len(params)} parameters but its body encodes {used}")
    return args


class Fmt:
    def __init__(self, path):
        text = strip_comments(re.sub(r"\\\r?\n", " ", path.read_text(encoding="utf-8")))
        defines = []
        for line in text.splitlines():
            m = DEFINE_RE.match(line.strip())
            if m:
                params = [p.strip() for p in m.group(2).split(",")] if m.group(2) is not None else None
                defines.append((m.group(1), params, (m.group(3) or "").strip()))
        codes = {}
        for name, params, body in defines:
            if name.startswith("MESSAGE_") and params is None and re.fullmatch(r"0[xX][0-9A-Fa-f]+|\d+", body):
                codes[name[len("MESSAGE_"):]] = int(body, 0)
        self.by_name, self.by_code, self.header = {}, {}, None
        for name, params, body in defines:
            if name in HELPER_MACROS or name.startswith("MESSAGE_"):
                continue
            params = params or []
            cm = CTRL_RE.match(body)
            if cm:
                if cm.group(1) not in codes:
                    raise ToolError(f"{path.name}: macro {name} uses CTRL_BASE({cm.group(1)}) with no MESSAGE_{cm.group(1)}")
                macro = Macro(name, params, codes[cm.group(1)], parse_macro_args(name, params, cm.group(2)))
                if macro.code in self.by_code:
                    raise ToolError(f"{path.name}: macros {self.by_code[macro.code].name} and {name} share code 0x{macro.code:02X}")
                self.by_name[name] = macro
                self.by_code[macro.code] = macro
            elif name == "HEADER":
                self.header = Macro(name, params, None, parse_macro_args(name, params, body))
        if "END" not in self.by_name:
            raise ToolError(f"{path.name}: no END macro")
        self.end_code = self.by_name["END"].code
        self.box_breaks = {n for n in self.by_name if n.startswith("BOX_BREAK")}
        em = re.search(r"enum\s+TextColor\s*\{(.*?)\}", text, re.S)
        self.colors = re.findall(r"TEXT_COLOR_(\w+)", em.group(1)) if em else []
        if any(k == "ARGC" for m in self.by_name.values() for k, _ in m.args) and not self.colors:
            raise ToolError(f"{path.name}: ARGC macro without a TextColor enum")


def load_glyphs(index):
    """byte -> glyph text from charmap.txt (newline is the NEWLINE control, not a glyph)."""
    charmap = ast.literal_eval((DECOMP / "assets/text/charmap.txt").read_text(encoding="utf-8"))
    return {v[index]: k for k, v in charmap.items() if v[index] is not None and k != "\n"}


def load_textbox_types():
    text = (DECOMP / "include/z64message.h").read_text(encoding="utf-8")
    m = re.search(r"typedef enum TextBoxType \{(.*?)\} TextBoxType;", text, re.S)
    names = {}
    if m:
        for n, name in re.findall(r"/\*\s*0x([0-9A-Fa-f]+)\s*\*/\s*(TEXTBOX_TYPE_\w+)", m.group(1)):
            names[int(n, 16)] = name
    return names


# ---------------------------------------------------------------------------------------------------------
# Messages

class Ctl:
    """A control code other than NEWLINE (newline is a "\\n" character in the text)."""
    __slots__ = ("name", "args")

    def __init__(self, name, args):
        self.name, self.args = name, args

    def label(self):
        return f"{self.name}({', '.join(self.args)})" if self.args else self.name


class Msg:
    def __init__(self, text_id, box_type, box_pos, header, tokens, controls):
        self.text_id, self.box_type, self.box_pos = text_id, box_type, box_pos
        self.header = header  # {field: int} or None
        self.tokens = tokens  # [str | Ctl]
        self.controls = controls  # Counter of control macro names, NEWLINE included

    def text(self):
        return "".join(t if isinstance(t, str) else f"[{t.label()}]" for t in self.tokens)

    def plain(self):
        return "".join(t for t in self.tokens if isinstance(t, str))

    def signature(self):
        names = tuple(t.name for t in self.tokens if not isinstance(t, str))
        hdr = tuple(self.header.items()) if self.header is not None else None
        return (self.box_type, self.box_pos, hdr, self.plain(), names)


def add_text(tokens, s):
    if tokens and isinstance(tokens[-1], str):
        tokens[-1] += s
    else:
        tokens.append(s)


def format_num(param, value, width):
    return f"0x{value:0{2 * width}X}" if param.endswith("Id") else str(value)


class Decoder:
    def __init__(self, kind, spec):
        self.kind = kind
        self.fmt = Fmt(DECOMP / spec["fmt"])
        self.glyphs = load_glyphs(spec["charmap_index"])

    def decode(self, text_id, box_type, box_pos, data):
        fmt = self.fmt
        if len(data) == 0:
            raise Undecodable("empty message")
        data = data.rstrip(b"\x00")  # alignment padding after END
        if not data or data[-1] != fmt.end_code:
            raise Undecodable(f"does not end with END (0x{fmt.end_code:02X})")
        data = data[:-1]
        pos = 0

        def take(n):
            nonlocal pos
            if pos + n > len(data):
                raise Undecodable("truncated control code argument")
            v = int.from_bytes(data[pos:pos + n], "big")
            pos += n
            return v

        header = None
        if fmt.header is not None:
            header = {}
            for kind, params in fmt.header.args:
                width = 2 if kind == "ARG2" else 1
                header[params[0]] = take(width)
        tokens, controls = [], Counter()
        while pos < len(data):
            b = data[pos]
            pos += 1
            macro = fmt.by_code.get(b)
            if macro is not None:
                controls[macro.name] += 1
                if macro.name == "NEWLINE":
                    add_text(tokens, "\n")
                    continue
                tokens.append(Ctl(macro.name, self.read_args(macro, take)))
            elif b in self.glyphs:
                add_text(tokens, self.glyphs[b])
            elif 0x20 <= b < 0x7F:
                add_text(tokens, chr(b))
            else:
                raise Undecodable(f"unrecognized control byte 0x{b:02X} at offset {pos - 1}")
        return Msg(text_id, box_type, box_pos, header, tokens, controls)

    def read_args(self, macro, take):
        out = []
        for kind, params in macro.args:
            if kind in ("ARG1", "ARG2"):
                w = 1 if kind == "ARG1" else 2
                out.append(format_num(params[0], take(w), w))
            elif kind == "ARGC":
                v = take(1)
                if v & 0xF0 != 0x40 or (v & 0xF) >= len(self.fmt.colors):
                    raise Undecodable(f"{macro.name}: bad color byte 0x{v:02X}")
                out.append(self.fmt.colors[v & 0xF])
            elif kind == "ARGB1":
                out.append(str(take(1)))
            else:  # ARGB2: (fgColor << 4 | bgColor), (yOffset << 4 | unk), per the fmt header macro
                b0, b1 = take(1), take(1)
                out += [str(b0 >> 4), str(b0 & 0xF), str(b1 >> 4), str(b1 & 0xF)]
        return out


def header_line(header):
    return "HEADER(" + ", ".join(f"0x{v:04X}" if k != "itemId" else f"0x{v:02X}" for k, v in header.items()) + ")"


def render_raw(msg, fmt):
    """The decoded message in the decomp's macro dialect (same layout rules as tools/text/msgdis.py)."""
    flat = []
    if msg.header is not None:
        flat.append(("HEADER", header_line(msg.header)))
    for t in msg.tokens:
        if isinstance(t, Ctl):
            flat.append((t.name, t.label()))
            continue
        for piece in re.split(r"(\n)", t):
            if piece == "\n":
                flat.append(("NEWLINE", "\\n"))
            elif piece:
                flat.append(("TEXT", piece.replace("\\", "\\\\").replace('"', '\\"')))
    if not flat:
        return '""'
    out, q, spaced, indent = "", False, False, 0

    def open_q():
        nonlocal out, q
        if not q:
            out, q = out + '"', True

    def close_q(space=False):
        nonlocal out, q
        if q:
            out += '" ' if space else '"'
            q = False

    for typ, dat in flat:
        if typ == "HEADER":
            out += dat + "\n"
            continue
        if typ in fmt.box_breaks:
            close_q()
            indent, spaced = 0, False
            out += "\n" + dat + "\n\n"
            continue
        if spaced:
            out += " "
            spaced = False
        if typ == "NEWLINE":
            open_q()
            out += '\\n"\n'
            q = False
            if indent:
                out += "    "
                indent -= 1
        elif typ == "TEXT":
            open_q()
            out += dat
        else:
            close_q(space=True)
            out += dat
            if typ in ("TWO_CHOICE", "THREE_CHOICE"):
                indent = 1 if typ == "TWO_CHOICE" else 2
                out += "\n    "
            else:
                spaced = True
    close_q()
    return out[:-1] if out.endswith("\n") else out


# ---------------------------------------------------------------------------------------------------------
# Binary side: message tables in the code segment, message data segments

def load_code_vram():
    with open(DECOMP / "baseroms" / VERSION / "segments.csv", newline="", encoding="utf-8") as f:
        for row in csv.reader(f):
            if row and row[0] == "code":
                return int(row[1], 16)
    raise ToolError("segments.csv has no code segment")


def load_variable(name):
    in_vars = False
    for line in (DECOMP / "baseroms" / VERSION / "config.yml").read_text(encoding="utf-8").splitlines():
        if line.startswith("variables:"):
            in_vars = True
        elif in_vars:
            m = re.match(r"^\s+(\w+):\s+(0[xX][0-9A-Fa-f]+)\s*(?:#.*)?$", line)
            if m and m.group(1) == name:
                return int(m.group(2), 16)
            if line and not line[0].isspace():
                in_vars = False
    raise ToolError(f"config.yml has no variable {name}")


def read_input(path):
    if not path.is_file():
        raise MissingInput(f"missing input {path}")
    return path.read_bytes()


def decode_binary(kind, spec):
    """Decode every message of one table -> (messages in table order, [(id, reason)] undecodable, [dup ids])."""
    code = read_input(SEGMENTS_DIR / "code")
    seg = read_input(SEGMENTS_DIR / spec["seg"])
    off = load_variable(spec["table"]) - load_code_vram()
    entries = []
    while True:
        if off < 0 or off + 8 > len(code):
            raise ToolError(f"{kind}: message table runs outside the code segment")
        text_id, info, addr = struct.unpack(">HBxI", code[off:off + 8])
        entries.append((text_id, info >> 4, info & 0xF, addr & ~0x0F000000))
        off += 8
        if text_id == END_OF_TABLE_ID:
            break
    decoder = Decoder(kind, spec)
    msgs, bad, seen, dups = [], [], set(), []
    for i in range(len(entries) - 1):
        text_id, box_type, box_pos, start = entries[i]
        end = entries[i + 1][3] if entries[i + 1][0] != END_OF_TABLE_ID else len(seg)
        if text_id in seen:
            dups.append(text_id)
        seen.add(text_id)
        if not 0 <= start <= end <= len(seg):
            bad.append((text_id, f"segment range 0x{start:X}..0x{end:X} outside 0x{len(seg):X}"))
            continue
        try:
            msgs.append(decoder.decode(text_id, box_type, box_pos, seg[start:end]))
        except Undecodable as e:
            bad.append((text_id, str(e)))
    return msgs, bad, dups, decoder.fmt


# ---------------------------------------------------------------------------------------------------------
# Source side: parse DEFINE_MESSAGE(...) macros (the stub files and, if present, extracted text)

TOK_RE = re.compile(r'\s*(?:(?P<str>"(?:\\.|[^\\"\n])*")|(?P<id>[A-Za-z_]\w*)|(?P<num>0[xX][0-9A-Fa-f]+|\d+)|(?P<p>\S))')
ESCAPES = {"n": "\n", "t": "\t", "r": "\r", "\\": "\\", '"': '"', "'": "'"}


def unescape(s):
    def sub(m):
        g = m.group(1)
        if len(g) == 3 and g[0] == "x":
            return chr(int(g[1:], 16))
        if g in ESCAPES:
            return ESCAPES[g]
        raise ToolError(f"unsupported escape \\{g} in a message string")
    return re.sub(r"\\(x[0-9A-Fa-f]{2}|.)", sub, s, flags=re.S)


def preprocess(path, missing, depth=0):
    """Inline quote-includes (current dir, then INCLUDE_DIRS), drop other directives and comments."""
    if depth > 8:
        raise ToolError(f"{path}: include depth exceeded")
    out = []
    for line in path.read_text(encoding="utf-8").splitlines():
        m = re.match(r'^\s*#\s*include\s+"([^"]+)"', line)
        if m:
            for d in [path.parent] + INCLUDE_DIRS:
                cand = d / m.group(1)
                if cand.is_file():
                    out.append(preprocess(cand, missing, depth + 1))
                    break
            else:
                missing.append(m.group(1))
        elif not line.lstrip().startswith("#"):
            out.append(line)
    return strip_comments("\n".join(out))


def parse_source(text, fmt, label):
    toks = [(m.lastgroup, m.group(m.lastgroup)) for m in TOK_RE.finditer(text)]
    pos = 0

    def peek(k=0):
        return toks[pos + k] if pos + k < len(toks) else (None, None)

    def expect(value):
        nonlocal pos
        if peek()[1] != value:
            raise ToolError(f"{label}: expected {value!r}, found {peek()[1]!r}")
        pos += 1

    def number():
        nonlocal pos
        kind, v = peek()
        if kind != "num":
            raise ToolError(f"{label}: expected a number, found {v!r}")
        pos += 1
        return int(v, 0)

    def arg_list():
        """Balanced '(' ... ')' -> list of top-level comma separated argument texts."""
        nonlocal pos
        expect("(")
        depth, args, cur = 1, [], []
        while True:
            kind, v = peek()
            if v is None:
                raise ToolError(f"{label}: unterminated argument list")
            pos += 1
            if v == "(":
                depth += 1
            elif v == ")":
                depth -= 1
                if depth == 0:
                    break
            elif v == "," and depth == 1:
                args.append(" ".join(cur))
                cur = []
                continue
            cur.append(v)
        if cur or args:
            args.append(" ".join(cur))
        return args

    msgs = []
    while pos < len(toks):
        kind, v = peek()
        if (kind, v) != ("id", "DEFINE_MESSAGE"):
            raise ToolError(f"{label}: unexpected token {v!r} outside DEFINE_MESSAGE")
        pos += 1
        expect("(")
        text_id = number()
        expect(",")
        box_type = number()
        expect(",")
        box_pos = number()
        expect(",")
        expect("MSG")
        expect("(")
        tokens, controls, header = [], Counter(), None
        while peek()[1] != ")":
            kind, v = peek()
            if kind is None:
                raise ToolError(f"{label}: unterminated MSG for 0x{text_id:04X}")
            if kind == "str":
                pos += 1
                add_text(tokens, unescape(v[1:-1]))
            elif kind == "id" and v == "HEADER":
                pos += 1
                args = arg_list()
                names = [p for _, ps in fmt.header.args for p in ps] if fmt.header else None
                if names is None or len(args) != len(names):
                    raise ToolError(f"{label}: HEADER takes {len(names or [])} args, got {len(args)} in 0x{text_id:04X}")
                try:
                    header = {n: int(a, 0) for n, a in zip(names, args)}
                except ValueError:
                    raise ToolError(f"{label}: non-numeric HEADER field in 0x{text_id:04X}")
            elif kind == "id":
                macro = fmt.by_name.get(v)
                if macro is None:
                    raise ToolError(f"{label}: unknown macro {v} in message 0x{text_id:04X}")
                pos += 1
                args = arg_list() if peek()[1] == "(" else []
                if len(args) != len(macro.params):
                    raise ToolError(f"{label}: macro {v} takes {len(macro.params)} args, got {len(args)} in 0x{text_id:04X}")
                controls[v] += 1
                if v == "NEWLINE":
                    add_text(tokens, "\n")
                elif v != "END":
                    tokens.append(Ctl(v, args))
            else:
                raise ToolError(f"{label}: unexpected token {v!r} in message 0x{text_id:04X}")
        expect(")")
        expect(")")
        msgs.append(Msg(text_id, box_type, box_pos, header, tokens, controls))
    return msgs


def load_source(kind, spec, fmt):
    missing = []
    stub = DECOMP / spec["stub"]
    if not stub.is_file():
        raise MissingInput(f"missing input {stub}")
    text = preprocess(stub, missing)
    return parse_source(text, fmt, spec["stub"]), missing


# ---------------------------------------------------------------------------------------------------------
# Build, validate, write

def entry_json(msg, fmt):
    text = msg.text()
    e = {"textId": f"{msg.text_id:04X}", "textboxType": msg.box_type, "textboxYPos": msg.box_pos}
    for k, v in (msg.header or {}).items():
        e[k] = f"{v:04X}" if k == "nextTextId" else v
    props = (msg.header or {}).get("textBoxProperties")
    if props is not None:  # the game reads the real box settings from here, see z_message.c
        e["propsHasChoices"] = (props & 0xF000) >> 12
        e["propsTextboxType"] = (props & 0x0F00) >> 8
        e["propsTextboxYPos"] = (props & 0x00F0) >> 4
    e["raw"] = render_raw(msg, fmt)
    e["text"] = text
    e["lineCount"] = text.count("\n") + 1 if text else 0
    return e


def build(kind):
    spec = SOURCES[kind]
    msgs, bad, dups, fmt = decode_binary(kind, spec)
    problems = [f"{kind}: duplicate id {i:04X}" for i in dups]
    problems += [f"{kind}: cannot decode {i:04X}: {why}" for i, why in bad]
    by_id = {m.text_id: m for m in msgs}
    src, missing = load_source(kind, spec, fmt)
    problems += [f"{kind}: duplicate id {i:04X} in source text" for i, n in Counter(s.text_id for s in src).items() if n > 1]
    checked = 0
    for s in src:
        d = by_id.get(s.text_id)
        if d is None:
            if s.text_id not in {i for i, _ in bad}:
                problems.append(f"{kind}: source message {s.text_id:04X} is not in the message table")
        elif s.signature() != d.signature():
            problems.append(f"{kind}: source and decode disagree for {s.text_id:04X}")
        else:
            checked += 1
    return dict(kind=kind, fmt=fmt, msgs=msgs, problems=problems, source_checked=checked, source_total=len(src),
                source_missing=missing)


def require_ignored():
    lines = [ln.strip() for ln in (ROOT / ".gitignore").read_text(encoding="utf-8").splitlines()]
    if "local-reference/" not in lines:
        raise ToolError(".gitignore does not list local-reference/; refusing to write Nintendo text")


def report_control_codes(results):
    per_file = {r["kind"]: Counter() for r in results}
    for r in results:
        for m in r["msgs"]:
            per_file[r["kind"]].update(m.controls)
    total = Counter()
    for c in per_file.values():
        total.update(c)
    return per_file, total


def do_build(write):
    require_ignored()
    results = [build(k) for k in SOURCES]
    problems = [p for r in results for p in r["problems"]]
    if write:
        OUT_DIR.mkdir(parents=True, exist_ok=True)
        for r in results:
            data = {f"{m.text_id:04X}": entry_json(m, r["fmt"]) for m in r["msgs"]}
            (OUT_DIR / SOURCES[r["kind"]]["out"]).write_text(
                json.dumps(data, ensure_ascii=False, indent=1) + "\n", encoding="utf-8", newline="\n")
    return results, problems


def print_totals(results, problems):
    per_file, total = report_control_codes(results)
    for r in results:
        k = r["kind"]
        note = f", {len(r['source_missing'])} include(s) not generated in tree" if r["source_missing"] else ""
        print(f"{k}: {len(r['msgs'])} entries, {len(per_file[k])} distinct control codes, "
              f"{r['source_checked']}/{r['source_total']} source messages agree with decode{note}")
    print(f"total: {sum(len(r['msgs']) for r in results)} entries, {len(total)} distinct control codes "
          f"(NEWLINE included), {len(problems)} problem(s)")


def print_stats(results):
    types = load_textbox_types()
    per_file, _ = report_control_codes(results)
    for r in results:
        k = r["kind"]
        print(f"[{k}] {len(r['msgs'])} messages")
        by_type = Counter(m.box_type for m in r["msgs"])
        for t, n in sorted(by_type.items()):
            print(f"  table textboxType 0x{t:02X} {types.get(t, '?')}: {n}")
        by_props = Counter((m.header["textBoxProperties"] & 0x0F00) >> 8 for m in r["msgs"] if m.header)
        for t, n in sorted(by_props.items()):
            print(f"  textBoxProperties type 0x{t:02X} {types.get(t, '?')}: {n}")
        print("  top control codes:")
        for name, n in per_file[k].most_common(20):
            print(f"    {name}: {n}")


def load_json(kind):
    path = OUT_DIR / SOURCES[kind]["out"]
    if not path.is_file():
        raise MissingInput(f"{path} not found; run the tool without arguments first")
    return json.loads(path.read_text(encoding="utf-8"))


def norm_id(s):
    s = s.strip()
    s = s[2:] if s.lower().startswith("0x") else s
    return s.upper().zfill(4)


def main(argv=None):
    if hasattr(sys.stdout, "reconfigure"):
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    ap = argparse.ArgumentParser(description="Extract Majora's Mask message text into ignored local JSON.")
    ap.add_argument("--id", help="print one entry (hex text id, e.g. 180B)")
    ap.add_argument("--grep", metavar="REGEX", help="print ids whose decoded text matches, with the first 60 chars")
    ap.add_argument("-i", "--ignore-case", action="store_true", help="case-insensitive --grep")
    ap.add_argument("--file", choices=sorted(SOURCES), help="restrict --id/--grep to one file")
    ap.add_argument("--stats", action="store_true", help="counts per textbox type and top control codes (no write)")
    args = ap.parse_args(argv)
    kinds = [args.file] if args.file else list(SOURCES)
    try:
        if args.id:
            want, found = norm_id(args.id), False
            for k in kinds:
                entry = load_json(k).get(want)
                if entry:
                    found = True
                    print(f"[{k}] {want}")
                    print(json.dumps(entry, ensure_ascii=False, indent=2))
            if not found:
                print(f"no entry {want}", file=sys.stderr)
            return 0 if found else 1
        if args.grep:
            rx = re.compile(args.grep, re.I if args.ignore_case else 0)
            for k in kinds:
                for tid, entry in load_json(k).items():
                    if rx.search(entry["text"]):
                        print(f"{k} {tid}  {entry['text'][:60].replace(chr(10), chr(92) + 'n')}")
            return 0
        results, problems = do_build(write=not args.stats)
        if args.stats:
            print_stats(results)
        print_totals(results, problems)
        if not args.stats:
            print(f"wrote {OUT_DIR / SOURCES['nes']['out']} and {OUT_DIR / SOURCES['staff']['out']}")
        for p in problems:
            print("FAIL " + p, file=sys.stderr)
        return 1 if problems else 0
    except MissingInput as e:
        print(f"ERROR: {e}", file=sys.stderr)
        return 2
    except ToolError as e:
        print(f"ERROR: {e}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
