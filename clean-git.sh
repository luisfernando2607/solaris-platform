#!/bin/bash

echo "--------------------------------------"
echo "LIMPIANDO REPO .NET"
echo "--------------------------------------"

if [ ! -d ".git" ]; then
  echo "❌ No estás dentro de un repositorio git"
  exit 1
fi

echo "✔ Repo detectado"

echo ""
echo "eliminando bin y obj..."

git rm -r --cached . 2>/dev/null

git add .

echo ""
echo "limpiando build..."

dotnet clean

echo ""
echo "estado final:"
git status

echo ""
echo "✔ REPO LIMPIO"
