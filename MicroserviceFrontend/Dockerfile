FROM node:latest as build

WORKDIR /usr/local/app

COPY ["MicroserviceFrontend/package.json", "."]

RUN npm install

COPY ["MicroserviceFrontend/", "."]

RUN npm run build

FROM nginx:latest

COPY --from=build /usr/local/app/dist/MicroserviceFrontend /usr/share/nginx/html

COPY ./../config/nginx.conf /etc/nginx/nginx.conf

EXPOSE 80