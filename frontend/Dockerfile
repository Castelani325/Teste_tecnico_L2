# Etapa 1: build (compila o React/Vite em arquivos estáticos)
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .

# VITE_API_URL precisa ser conhecida em tempo de BUILD (o Vite embute
# variáveis de ambiente no código estático na hora de compilar, não em runtime)
ARG VITE_API_URL=http://localhost:5001
ENV VITE_API_URL=$VITE_API_URL
RUN npm run build

# Etapa 2: servir os arquivos estáticos gerados
FROM nginx:alpine AS final
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
