FROM node:20-alpine AS base

ENV HUSKY=0

FROM base AS deps
WORKDIR /app

COPY package.json package-lock.json* ./
RUN npm ci

FROM base AS dev
WORKDIR /app

COPY --from=deps /app/node_modules ./node_modules
COPY . .

FROM dev AS builder

RUN npm run build

FROM nginx:alpine AS runner

COPY --from=builder /app/default.conf.template /etc/nginx/templates/default.conf.template
COPY --from=builder /app/dist /usr/share/nginx/html
COPY --from=builder /app/entrypoint.sh /docker-entrypoint.d/entrypoint.sh

RUN ["chmod", "755", "/docker-entrypoint.d/entrypoint.sh"]

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]