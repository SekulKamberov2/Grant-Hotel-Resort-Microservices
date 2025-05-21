# Stage 1: Build the React app
FROM node:18 as build

# Set the working directory inside the container
WORKDIR /app

# Copy the package.json and package-lock.json files
COPY package.json ./
COPY package-lock.json ./

# Install dependencies
RUN npm install

# Copy the rest of the React app's source code
COPY . ./

# Build the React app for production
RUN npm run build

# Stage 2: Serve the build with Nginx
FROM nginx:alpine

# Copy the build output from the previous build stage into Nginx's public folder
COPY --from=build /app/build /usr/share/nginx/html

# Copy a custom Nginx configuration file (you must update it to listen on port 90)
COPY nginx.conf /etc/nginx/nginx.conf

# Expose port 90 instead of 80
EXPOSE 90

# Command to run Nginx in the foreground
CMD ["nginx", "-g", "daemon off;"]
