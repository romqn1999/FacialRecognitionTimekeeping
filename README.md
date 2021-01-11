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
