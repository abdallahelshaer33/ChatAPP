# استخدم صورة الـ SDK لبناء التطبيق
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# انسخ كل الملفات وبنّي المشروع
COPY . ./
RUN dotnet publish -c Release -o out

# صورة خفيفة لتشغيل التطبيق
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# شغّل التطبيق
ENTRYPOINT ["dotnet", "ChatApplicationYT.dll"]
