# Stage 1: Compile and Build angular codebase

# Use official node image as the base image
FROM node:latest as build

# Set the working directory
WORKDIR /usr/local/app

# copy package.json
COPY ["MicroserviceFrontend/package.json", "."]
#COPY ["MicroserviceFrontend/package-lock.json", "."]

# Install all the dependencies
RUN npm install

#copy source files
COPY ["MicroserviceFrontend/", "."]

# Generate the build of the application
RUN npm run build


# Stage 2: Serve app with nginx server

# Use official nginx image as the base image
FROM nginx:latest

# Copy the build output to replace the default nginx contents.
COPY --from=build /usr/local/app/dist/MicroserviceFrontend /usr/share/nginx/html

#copy nginx config file to send all routes to index.html
COPY ./../config/nginx.conf /etc/nginx/nginx.conf

# Expose port 80
EXPOSE 80