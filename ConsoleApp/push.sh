#!/bin/bash

SSH_DIR=/root/.ssh
NFS_DIR=/app/files
GIT_REPO="$GIT_REPO"
GIT_WORKDIR="$GIT_WORKDIR"
REPO_NAME=$(basename $(basename $GIT_REPO) .git)

#設置 SSH 金鑰
mkdir -p "$SSH_DIR"
env | grep GITHUB_KEY | cut -d = -f 2 > "$SSH_DIR/id_rsa"
echo -e $(cat "$SSH_DIR/id_rsa") > "$SSH_DIR/id_rsa"
env | grep GITHUB_PUBKEY | cut -d = -f 2 > "$SSH_DIR/id_rsa.pub"
chmod 400 "$SSH_DIR/id_rsa"
ssh-keyscan github.com >> "$SSH_DIR/known_hosts"

#啟動 SSH proxy 並添加金鑰
eval "$(ssh-agent -s)"
ssh-add "$SSH_DIR/id_rsa"

#確認是否有 Git 儲存庫
if [ ! -d "$NFS_DIR/wwwroot/.git" ]; then
  git clone "$GIT_REPO" "$NFS_DIR"
  mv "$NFS_DIR/$REPO_NAME" "$NFS_DIR/wwwroot"
fi

#更新資料
cd "$NFS_DIR/wwwroot"
git config --global user.email "robot@moda.gov.tw"
git config --global user.name "robot"
git config pull.rebase false
git pull

#推送更新到 remote repository
git remote set-url origin "$GIT_REPO"
git add .
git commit -m "robotUpdate"
git push origin main
