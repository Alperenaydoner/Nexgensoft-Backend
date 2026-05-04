# Nexgensoft-Backend

ASP.NET Core **CoreService** API (SQL Server, EF Core migrations).

## Yerel kurulum

1. `CoreService/appsettings.Development.example.json` dosyasını kopyalayıp `appsettings.Development.json` yapın; bağlantı dizesini kendi sunucunuza göre düzenleyin (bu dosya **git’e girmez**).
2. `cd CoreService`
3. `dotnet restore`
4. `dotnet ef database update` (ilk kez veya migration sonrası)
5. `dotnet run --launch-profile https`

## GitHub

Bu klasör ayrı bir repo köküdür. Uzak repo adı: **Nexgensoft-Backend**.

```bash
git remote add origin https://github.com/Alperenaydoner/Nexgensoft-Backend.git
git branch -M main
git push -u origin main
```
