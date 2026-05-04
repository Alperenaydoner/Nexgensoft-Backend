# Nexgensoft-Backend

ASP.NET Core **CoreService** API (PostgreSQL / Supabase, EF Core migrations).

## Yerel kurulum

1. `CoreService/appsettings.Development.example.json` dosyasını kopyalayıp `appsettings.Development.json` yapın; Supabase **Database** şifresini `Password=` alanına yazın (bu dosya **git’e girmez**).
2. `cd CoreService`
3. `dotnet restore`
4. `dotnet ef database update` (Supabase Postgres şemasını oluşturur)
5. `dotnet run --launch-profile https`

Üretimde bağlantıyı ortam değişkeni ile verin: `ConnectionStrings__DefaultConnection` (tam Npgsql connection string; `SSL Mode=Require` kullanın).

## Supabase / DNS (Windows)

`System.Net.Sockets.SocketException` — *“İstenen ad geçerli olduğu halde istenen türde bir veri bulunamadı”* — çoğunlukla **IPv6 (AAAA)** kaydı varken makinede IPv6 rotası olmamasından kaynaklanır. `appsettings.Development.json` içinde **`Database:PreferIpv4Dns": true`** (varsayılan örnekte açık) ile uygulama migration / DB bağlantısından hemen önce IPv4 tercih edilir.

Host, port, kullanıcı adı (`postgres.<project_ref>`) ve parola **Supabase Dashboard → Database** ile aynı olsun.

- **Port 6543** = *transaction* pooler (PgBouncer): kısa sorgular için uygundur; **EF Core migration** sırasında akış zaman aşımı / “transient failure” üretebilir.
- **Port 5432** (aynı `*.pooler.supabase.com` host’u) = *session* pooler: ORM ve `MigrateAsync` için genelde doğru seçim.
- Hâlâ sorun varsa **direct** bağlantı: `db.<ref>.supabase.co:5432`, kullanıcı `postgres` (Dashboard’daki “Direct connection” dizesini kopyalayın).

## GitHub

Bu klasör ayrı bir repo köküdür. Uzak repo adı: **Nexgensoft-Backend**.

```bash
git remote add origin https://github.com/Alperenaydoner/Nexgensoft-Backend.git
git branch -M main
git push -u origin main
```

