# Hospital Management System (C# WinForms + SQL Server)

Ky projekt eshte nje **Hospital Management System** edukativ/profesional per nivel shkollor, i ndertuar me:

- C# Windows Forms
- SQL Server
- ADO.NET (Microsoft.Data.SqlClient)

## Modulet e perfshira

1. **Login System** (Admin, Doctor, Receptionist)
2. **Patients** (CRUD + Search + DataGridView)
3. **Doctors** (CRUD)
4. **Appointments** (CRUD + status)
5. **Medical Records** (CRUD)
6. **Billing** (CRUD + Paid/Unpaid)
7. **Rooms** (CRUD + Available/Occupied)
8. **Dashboard** (total pacient, total doktor, termine sot, fatura te papaguara)

---

## Struktura e projektit

- `HospitalManagementSystem/Program.cs` - pika e nisjes
- `HospitalManagementSystem/Forms/LoginForm.cs` - forma e login
- `HospitalManagementSystem/Forms/MainForm.cs` - paneli kryesor me te gjitha modulet
- `HospitalManagementSystem/Data/DbManager.cs` - helper per lidhje/query SQL
- `HospitalManagementSystem/Models/Entities.cs` - modelet kryesore
- `HospitalManagementSystem/Sql/HospitalManagementDB.sql` - script i databazes + sample data

---

## Hapat per setup dhe ekzekutim

## 1) Krijo databazen

1. Hap **SQL Server Management Studio (SSMS)**.
2. Hap file: `HospitalManagementSystem/Sql/HospitalManagementDB.sql`.
3. Ekzekuto script-in (Run / F5).

Kjo krijon databazen `HospitalManagementDB`, tabelat, foreign keys, disa stored procedures dhe sample data.

## 2) Konfiguro connection string

Hap `HospitalManagementSystem/Data/DbManager.cs` dhe kontrollo kete rresht:

```csharp
private const string ConnectionString = "Server=.;Database=HospitalManagementDB;Trusted_Connection=True;TrustServerCertificate=True;";
```

- Nese serveri yt nuk eshte `.` (local default), ndrysho `Server=...` sipas rastit, p.sh.:
  - `Server=DESKTOP-XXXX\SQLEXPRESS;...`

## 3) Hap projektin ne Visual Studio

1. Hap `HospitalManagementSystem.sln` ne Visual Studio 2022.
2. Sigurohu qe target framework eshte i suportuar (`net8.0-windows`).
3. Build Solution.
4. Run (F5).

## 4) Kredencialet per login (sample)

- **Admin**: `admin` / `admin123`
- **Doctor**: `doctor1` / `doctor123`
- **Receptionist**: `reception` / `recep123`

---

## Validime dhe sjellje te aplikacionit

- Input validation ne format kryesore (fushat e detyrueshme).
- Mesazhe suksesi/gabimi me `MessageBox`.
- Data shfaqet ne `DataGridView` per secilin modul.
- CRUD me query parametrike (mbrojtje baze nga SQL Injection).

---

## Ide per permiresim

- Hashing i password (BCrypt/ASP.NET Identity pattern).
- Repository pattern dhe shtresa service.
- Logging dhe audit trail.
- Raporte PDF per faturat.
