find_source_files(GLTFSerialization_SOURCE_FILES "${GLTFSerialization_repository_root}/GLTFSerialization")
remove_paths_from_list(GLTFSerialization_SOURCE_FILES "${GLTFSerialization_repository_root}/GLTFSerialization/obj")


add_definitions(-DUINEST -DON_WINDOWS -DUNITY_STANDALONE_WIN -DUNITY_5_5_OR_NEWER -DUNITY_2017_1_OR_NEWER)

add_library(GLTFSerialization SHARED

    ${GLTFSerialization_SOURCE_FILES}
)

set_property(TARGET GLTFSerialization PROPERTY VS_DOTNET_REFERENCES

    "Microsoft.CSharp"
    "System"
    "System.Core"
    "System.Collections.Immutable"
    "${GLTFSerialization_repository_root}/../UnityGLTF/Assets/Plugins/Newtonsoft.Json.dll"
    )
set_property(TARGET GLTFSerialization PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.7.2")


set(CMAKE_CSharp_FLAGS "/platform:anycpu")
target_compile_options(GLTFSerialization PUBLIC "/unsafe")