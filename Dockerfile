FROM mono:6.12

# 1. 修正來源並安裝工具
RUN sed -i 's/deb.debian.org/archive.debian.org/g' /etc/apt/sources.list && \
    sed -i 's/security.debian.org/archive.debian.org/g' /etc/apt/sources.list && \
    sed -i '/stretch-updates/d' /etc/apt/sources.list && \
    apt-get update && \
    apt-get install -y mono-roslyn mono-xsp4 nuget mono-complete sed && \
    rm -rf /var/lib/apt/lists/*

# 2. 建立 Hugging Face 指定的使用者 (UID 1000)
RUN useradd -m -u 1000 user
WORKDIR /home/user/app

# 3. 複製所有檔案並修改擁有者
COPY . .
RUN chown -R user:user /home/user/app

# 4. 編譯
RUN cert-sync /etc/ssl/certs/ca-certificates.crt && \
    nuget restore DatabaseQuiz.sln && \
    msbuild /p:Configuration=Release DatabaseQuiz.sln

# 5. 設定 Port (Hugging Face 規定必須是 7860)
EXPOSE 7860

# 6. 切換使用者
USER user

# 7. 啟動並替換密碼 (DB_PASSWORD 會從 Secrets 抓取)
CMD sed -i "s/YOUR_PASSWORD/$DB_PASSWORD/g" Web.config && \
    xsp4 --port 7860 --nonstop --root .