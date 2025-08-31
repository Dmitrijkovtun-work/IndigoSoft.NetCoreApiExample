API_SOLUTION=NetCoreApiExample.sln
API_ARTIFACT_PATH=backend_release
API_PUBLISH_PATH=Api/bin/Release/net8.0/publish
API_IMAGE_NAME=net-core-api-example
API_IMAGE_TAG=rev0.1

echo -e "Compiling api ..."
cd ../src/backend
dotnet restore ${API_SOLUTION}
dotnet publish --no-restore ${API_SOLUTION} -c Release 

echo -e "Build image api ..."
cp ../../deploy/dockerfile ${API_PUBLISH_PATH}/dockerfile
docker build -t ${API_IMAGE_NAME}:${API_IMAGE_TAG} ${API_PUBLISH_PATH}

cd ../../deploy
echo -e "\n\n\e[31mWARNING\e[0m: Docker create own chain in IPTABLES after up container, used ports will be available from internet !!!"

read -n 1 -p "Up indigo-soft-example-api (y/n) ?" isYes
    case $isYes in
    y)
        echo -e "Up container ...\n"
        docker-compose -p indigo-soft-example-api up
    ;;
    *)
        echo -e "\n\n  Nooo"
    ;;
    esac
