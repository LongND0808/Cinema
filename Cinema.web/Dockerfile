# Sử dụng hình ảnh chính thức của .NET 6 SDK để xây dựng ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Sao chép các file csproj và khôi phục các phụ thuộc
COPY *.csproj ./
RUN dotnet restore

# Sao chép toàn bộ mã nguồn và xây dựng ứng dụng
COPY . ./
RUN dotnet publish -c Release -o out

# Sử dụng hình ảnh chính thức của .NET 6 Runtime để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Cấu hình cổng mà ứng dụng sẽ lắng nghe
EXPOSE 44332

ENTRYPOINT ["dotnet", "Cinema.web.dll"]
