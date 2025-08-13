# Use Node.js 18 Alpine as base image for building
FROM node:18-alpine AS build

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install all dependencies (including dev dependencies needed for build)
RUN npm ci

# Copy source code
COPY . .

# Set environment variable to indicate Docker build
ENV DOCKER=true

# Build the application
RUN npm run build

# Use nginx to serve the built application
FROM nginx:alpine AS final

# Copy the built application from the build stage
COPY --from=build /app/dist /usr/share/nginx/html

# Remove default nginx config and copy our custom configuration
RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/nginx.conf

# Create symlink to fix nginx path issue
RUN mkdir -p /etc/nginx && ln -sf /usr/share/nginx/html /etc/nginx/html

# Expose port 3000
EXPOSE 3000

# Start nginx
CMD ["nginx", "-g", "daemon off;"]