# Step 1: Build the React app
FROM node:18-alpine AS build

WORKDIR /app

# Copy package.json and install dependencies
COPY package.json package-lock.json ./
RUN npm install

COPY . .

RUN npm run build

FROM nginx:alpine

ENV PORT=3003

RUN rm /etc/nginx/conf.d/default.conf

COPY nginx.conf /etc/nginx/conf.d

COPY --from=build /app/build /usr/share/nginx/html

EXPOSE 3003

CMD ["nginx", "-g", "daemon off;"]
