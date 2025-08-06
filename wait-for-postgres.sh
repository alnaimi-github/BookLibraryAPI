#!/bin/sh
until pg_isready -h book-lib-postgres -p 5432 -U postgres; do
  echo "Waiting for postgres..."
  sleep 2
done
exec "$@"
