FROM auriga-repo.nts.local:5001/nginx:alpine

RUN rm -f /etc/nginx/conf.d/default.conf; \
  mkdir -p /var/cache/nginx; \
  chown -R nginx:nginx /etc/nginx/ /usr/share/nginx /var/log/nginx /var/cache/nginx

COPY ./artifacts/apps_cumlist /usr/share/nginx/html/app_cumlist
RUN ls -la /usr/share/nginx/html/

COPY ./nginx.conf /etc/nginx/nginx.conf