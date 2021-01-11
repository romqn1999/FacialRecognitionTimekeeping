# FacialRecognitionTimekeeping

# Hệ thống điểm danh bằng nhận diện mặt
    - Project này là API server cho hệ thống điểm danh bằng nhận diện mặt
    - Hệ thống gồm 4 function chính:
        - Đăng kí thông tin, mặt
        - Ghi nhận điểm danh bằng ảnh chứa mặt
        - Xem lịch sử các lần điểm danh
        - Xóa thông tin một người
## Đăng kí
    - Truyền vào:
        - Thông tin là tên định danh phân biệt (không trùng)
        - Một tấm ảnh chứa mặt, nếu có nhiều mặt trong ảnh, hệ thống sẽ lấy mặt có kích thước lớn nhất
## Ghi nhận điểm danh
    - Truyền vào:
        - Một tấm ảnh chứa một hoặc nhiều mặt
    - Hệ thống sẽ nhận diện các gương mặt có trong ảnh và ghi nhận thời gian điểm danh cho các mặt nhận diện được
## Xem lịch sử điểm danh
    - Hệ thống cho phép xem thời gian của các lần đã điểm danh
## Xóa
    - Hệ thống cho phép xóa thông tin gồm cả tên, mặt và lịch sử điểm danh của một người nào đó

# Công nghệ
    - .Net Core MVC: framework build api
    - SQL Server: database
    - Cognitive azure service: nhận diện mặt
    - Docker
    - Can migration database
    - Pipeline design pattern

# Build & Run
Build & Run with `docker-compose.yml` file.
## Build
```shell
    $docker-compose build
```
## Run
```shell
    $docker-compose up
```
or
```shell
    $docker-compose up -d
```

# HTTPS
- A certificate from a certificate authority is required for production hosting for a domain. Creating a Certificate to use ASP .Net Core with HTTPS in Docker:
    ```shell
        $dotnet dev-certs https -ep .aspnet\https\aspnetapp.pfx -p dacrom
        $dotnet dev-certs https --trust
    ```
- We share the certificate file with our docker container as in `docker-compose.yml` file:
    ```
    environment:
      - ASPNETCORE_URLS=https://+:443;http://+:80
      - ASPNETCORE_Kestrel__Certificates__Default__Password=dacrom
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx
    volumes:
      - ./.aspnet/https:/https:ro
    ```

# Issues
- `ERROR: BootstrapSystemDataDirectories() failure (HRESULT 0x80070002)`. Fix:
    > Run mssql as same user as user & group of mount directory of container.
    > Ex: current user
    ```shell
    $CURRENT_UID=$(id -u):$(id -g) docker-compose up
    ```
    > Ex: *dacle*
    ```shell
    $CURRENT_UID=$(id -u dacle):$(id -g dacle) docker-compose up
    ```
- `/opt/mssql/bin/sqlservr: Error: The system directory [/.system] could not be created.  Errno [13]`. Fix:
    ```shell
    $CURRENT_UID=$(id -u root):$(id -g root) docker-compose up
    ```

# Database SQL Server
- Connect to SQL Server container: https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver15&pivots=cs1-bash#connect-to-sql-server

## Add migration when model changes
- Command for `PowerShell` or `Packag Manager Console` in Visual Studio 
    ```shell
    PM> Add-Migration InitDatabase
    ```
