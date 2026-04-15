# 使用基於 Ubuntu 的 Mono 鏡像
FROM mono:6.12

# 1. 修正 Debian 存檔位址並安裝工具
RUN sed -i 's/deb.debian.org/archive.debian.org/g' /etc/apt/sources.list && \
    sed -i 's/security.debian.org/archive.debian.org/g' /etc/apt/sources.list && \
    sed -i '/stretch-updates/d' /etc/apt/sources.list && \
    apt-get update && \
    apt-get install -y \
    mono-roslyn \
    mono-xsp4 \
    nuget \
    mono-complete \
    sed && \
    rm -rf /var/lib/apt/lists/*

# 2. 設定工作目錄
WORKDIR /app

# 3. 複製專案檔案
COPY . .

# 4. 還原 NuGet 套件 (已改為 DatabaseQuiz.sln)
RUN cert-sync /etc/ssl/certs/ca-certificates.crt && \
    nuget restore DatabaseQuiz.sln

# 5. 編譯專案 (已改為 DatabaseQuiz.sln)
RUN msbuild /p:Configuration=Release DatabaseQuiz.sln

# 6. 暴露 Port
EXPOSE 8080

# 7. 啟動伺服器
# 啟動前先執行 sed 指令，將 Web.config 裡的 YOUR_PASSWORD 替換成環境變數 $DB_PASSWORD
# 這是 Dockerfile 的最後一部分
# 它會把 Web.config 裡的 YOUR_PASSWORD 換成 Render 上的 $DB_PASSWORD 變數
CMD sed -i "s/YOUR_PASSWORD/$DB_PASSWORD/g" Web.config && \
    xsp4 --port 8080 --nonstop --root .