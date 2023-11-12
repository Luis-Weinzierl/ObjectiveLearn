version="1.0"
revision="1"

if ! command -v dotnet &> /dev/null
then
    echo ".NET is not installed. Please install the .NET-SDK 6.0 or higher and try again."
    exit 1
fi

if ! command -v dpkg-deb &> /dev/null
then
    echo "dpkg-deb is not installed. Please install the dpkg-Toolset and try again."
    exit 1
fi


while getopts v:r: flag
do
    case "${flag}" in
        v) version=${OPTARG};;
        r) revision=${OPTARG};;
    esac
done

declare -A build_platforms
build_platforms[amd64]="ubuntu.16.04-x64"
build_platforms[arm64]="ubuntu.16.04-arm64"
build_platforms[x64]="ubuntu.16.04-x64"

mkdir pkgbin
rm -r pkgbin/*

for key in "${!build_platforms[@]}"
do
    runtime="${build_platforms[$key]}"
    out_folder="pkgbin/objectivelearn_$version-${revision}_$key"
    out_path="${out_folder}/usr/bin/Files.ObjectiveLearn/"
    control_file_path="${out_folder}/DEBIAN/control"
    control_file="Package: objectivelearn\nVersion: $version-$revision\nMaintainer: Luis Weinzierl\nArchitecture: $key\nDescription: A teaching-program for the Werdenfels-Gymnasium\n"

    echo "Building Platform: $key"

    cp -r objectivelearn_debtemplate "${out_folder}"

    printf "${control_file}" > "${control_file_path}"

    dotnet publish ObjectiveLearn -p:Platform=Linux -c:Release -r:$runtime -o:$out_path --self-contained

    dpkg-deb --root-owner-group --build "${out_folder}/"
done

echo "Done."