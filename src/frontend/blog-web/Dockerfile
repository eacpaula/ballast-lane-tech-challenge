FROM node:22-alpine

WORKDIR /app

COPY src/frontend/blog-web/package*.json ./

RUN npm ci

COPY src/frontend/blog-web/ ./

EXPOSE 5173

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "5173"]
