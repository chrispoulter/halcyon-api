FROM node:22-alpine AS build

WORKDIR /app

COPY package.json package.json
COPY package-lock.json package-lock.json

RUN npm install

COPY . .

RUN npm run build

FROM nginx:alpine AS final

COPY --from=builder /app/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=builder /app/dist /usr/share/nginx/html
COPY --from=builder /app/entrypoint.sh /docker-entrypoint.d/entrypoint.sh

RUN ["chmod", "755", "/docker-entrypoint.d/entrypoint.sh"]

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]