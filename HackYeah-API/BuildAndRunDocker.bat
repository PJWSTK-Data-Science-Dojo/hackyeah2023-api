docker stop hackyeah-api
docker rm hackyeah-api
docker rmi hackyeah-api
docker build -t hackyeah-api .
docker run -d -p 6969:80 --name hackyeah-api hackyeah-api