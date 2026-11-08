cmake_minimum_required(VERSION 3.20)
project(ZAPDNativeLocal LANGUAGES C CXX)
set(CMAKE_C_STANDARD 11)
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_MSVC_RUNTIME_LIBRARY "MultiThreaded$<$<CONFIG:Debug>:Debug>")
get_filename_component(ZAPD_ROOT "${CMAKE_CURRENT_LIST_DIR}/.." ABSOLUTE)
set(PNG_ROOT "${ZAPD_ROOT}/packages/libpng.static.1.6.37/build/native/png")
set(ZLIB_ROOT "${ZAPD_ROOT}/packages/zlib.static.1.2.5/build/native")
file(GLOB PNG_SRC CONFIGURE_DEPENDS "${PNG_ROOT}/*.c")
file(GLOB ZLIB_SRC CONFIGURE_DEPENDS "${ZLIB_ROOT}/src/*.c")
file(GLOB_RECURSE ZAPD_SRC CONFIGURE_DEPENDS "${ZAPD_ROOT}/ZAPD/*.cpp")
file(GLOB_RECURSE UTIL_SRC CONFIGURE_DEPENDS "${ZAPD_ROOT}/ZAPDUtils/*.cpp")
file(GLOB EXPORT_SRC CONFIGURE_DEPENDS "${ZAPD_ROOT}/ExporterTest/*.cpp")
set(GFXD_SRC
  "${ZAPD_ROOT}/lib/libgfxd/gfxd.c"
  "${ZAPD_ROOT}/lib/libgfxd/uc_f3d.c"
  "${ZAPD_ROOT}/lib/libgfxd/uc_f3db.c"
  "${ZAPD_ROOT}/lib/libgfxd/uc_f3dex.c"
  "${ZAPD_ROOT}/lib/libgfxd/uc_f3dexb.c"
  "${ZAPD_ROOT}/lib/libgfxd/uc_f3dex2.c")
file(WRITE "${CMAKE_CURRENT_BINARY_DIR}/BuildInfo.cpp" "extern const char gBuildHash[] = \"mm-3be2e241-native-msvc-local\";\n")
add_executable(ZAPDNative ${ZAPD_SRC} ${UTIL_SRC} ${EXPORT_SRC} ${GFXD_SRC}
  ${PNG_SRC} ${ZLIB_SRC} "${ZAPD_ROOT}/lib/tinyxml2/tinyxml2.cpp"
  "${CMAKE_CURRENT_BINARY_DIR}/BuildInfo.cpp")
target_include_directories(ZAPDNative PRIVATE
  "${ZAPD_ROOT}/ZAPD" "${ZAPD_ROOT}/ZAPDUtils" "${ZAPD_ROOT}/lib/tinyxml2"
  "${ZAPD_ROOT}/lib/libgfxd" "${ZAPD_ROOT}/lib/elfio" "${ZAPD_ROOT}/lib/stb"
  "${PNG_ROOT}" "${ZLIB_ROOT}/include" "${ZLIB_ROOT}/src")
target_compile_definitions(ZAPDNative PRIVATE _CRT_SECURE_NO_WARNINGS NOMINMAX PNG_IMPEXP=)
target_compile_options(ZAPDNative PRIVATE $<$<COMPILE_LANGUAGE:CXX>:/EHsc>)
target_link_libraries(ZAPDNative PRIVATE Dbghelp)
