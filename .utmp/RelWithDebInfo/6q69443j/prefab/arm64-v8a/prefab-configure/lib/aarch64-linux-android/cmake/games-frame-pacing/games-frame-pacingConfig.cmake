if(NOT TARGET games-frame-pacing::swappy_static)
add_library(games-frame-pacing::swappy_static STATIC IMPORTED)
set_target_properties(games-frame-pacing::swappy_static PROPERTIES
    IMPORTED_LOCATION "C:/Users/chudi/.gradle/caches/8.10.1/transforms/4fbb37456f720b511fcd1a6e46e93a54/transformed/jetified-games-frame-pacing-1.10.0/prefab/modules/swappy_static/libs/android.arm64-v8a_API23_NDK23_cpp_shared_Release/libswappy.a"
    INTERFACE_INCLUDE_DIRECTORIES "C:/Users/chudi/.gradle/caches/8.10.1/transforms/4fbb37456f720b511fcd1a6e46e93a54/transformed/jetified-games-frame-pacing-1.10.0/prefab/modules/swappy_static/include"
    INTERFACE_LINK_LIBRARIES ""
)
endif()

