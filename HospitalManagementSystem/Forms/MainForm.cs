using System.Data;
using Microsoft.Data.SqlClient;
using HospitalManagementSystem.Data;

namespace HospitalManagementSystem.Forms;

public class MainForm : Form
{
    private readonly string role;
    private readonly string username;
    private readonly TabControl tabControl = new() { Dock = DockStyle.Fill };

    private readonly Label lblPatients = new();
    private readonly Label lblDoctors = new();
    private readonly Label lblTodayAppointments = new();
    private readonly Label lblUnpaidBills = new();

    private readonly DataGridView dgvPatients = CreateGrid();
    private readonly DataGridView dgvDoctors = CreateGrid();
    private readonly DataGridView dgvAppointments = CreateGrid();
    private readonly DataGridView dgvRecords = CreateGrid();
    private readonly DataGridView dgvBills = CreateGrid();
    private readonly DataGridView dgvRooms = CreateGrid();

    private readonly TextBox txtPatientName = new();
    private readonly TextBox txtPatientPhone = new();
    private readonly TextBox txtPatientAddress = new();
    private readonly DateTimePicker dtpPatientDob = new() { Format = DateTimePickerFormat.Short };
    private readonly TextBox txtPatientSearch = new();

    private readonly TextBox txtDoctorName = new();
    private readonly TextBox txtSpecialization = new();
    private readonly TextBox txtDoctorPhone = new();
    private readonly TextBox txtDoctorEmail = new();
    private readonly TextBox txtDoctorSchedule = new();

    private readonly ComboBox cmbAppointmentPatient = new();
    private readonly ComboBox cmbAppointmentDoctor = new();
    private readonly DateTimePicker dtpAppointment = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm" };
    private readonly ComboBox cmbAppointmentStatus = new();

    private readonly ComboBox cmbRecordAppointment = new();
    private readonly TextBox txtDiagnosis = new();
    private readonly TextBox txtSymptoms = new();
    private readonly TextBox txtMedicines = new();
    private readonly TextBox txtDoctorNotes = new();

    private readonly ComboBox cmbBillPatient = new();
    private readonly TextBox txtBillService = new();
    private readonly TextBox txtBillPrice = new();
    private readonly ComboBox cmbBillStatus = new();

    private readonly TextBox txtRoomNumber = new();
    private readonly TextBox txtRoomType = new();
    private readonly ComboBox cmbRoomStatus = new();

    public MainForm(string role, string username)
    {
        this.role = role;
        this.username = username;

        Text = $"Hospital Management System - {role}: {username}";
        Width = 1250;
        Height = 760;
        StartPosition = FormStartPosition.CenterScreen;

        Controls.Add(tabControl);

        tabControl.TabPages.Add(CreateDashboardTab());
        tabControl.TabPages.Add(CreatePatientsTab());
        tabControl.TabPages.Add(CreateDoctorsTab());
        tabControl.TabPages.Add(CreateAppointmentsTab());
        tabControl.TabPages.Add(CreateRecordsTab());
        tabControl.TabPages.Add(CreateBillingTab());
        tabControl.TabPages.Add(CreateRoomsTab());

        LoadData();
        ApplyRolePermissions();
    }

    private static DataGridView CreateGrid() => new()
    {
        Dock = DockStyle.Bottom,
        Height = 360,
        ReadOnly = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };

    private TabPage CreateDashboardTab()
    {
        TabPage tab = new("Dashboard");
        FlowLayoutPanel panel = new() { Dock = DockStyle.Fill, Padding = new Padding(25), AutoScroll = true };

        panel.Controls.Add(CreateMetricCard("Total Paciente", lblPatients));
        panel.Controls.Add(CreateMetricCard("Total Doktore", lblDoctors));
        panel.Controls.Add(CreateMetricCard("Termine Sot", lblTodayAppointments));
        panel.Controls.Add(CreateMetricCard("Fatura Pa Paguar", lblUnpaidBills));

        tab.Controls.Add(panel);
        return tab;
    }

    private static Panel CreateMetricCard(string title, Label valueLabel)
    {
        Panel panel = new() { Width = 250, Height = 150, BackColor = Color.WhiteSmoke, Margin = new Padding(12) };
        Label lblTitle = new() { Text = title, Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(16, 22), AutoSize = true };
        valueLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
        valueLabel.Location = new Point(16, 60);
        valueLabel.AutoSize = true;
        panel.Controls.Add(lblTitle);
        panel.Controls.Add(valueLabel);
        return panel;
    }

    private TabPage CreatePatientsTab()
    {
        TabPage tab = new("Patients");
        Panel top = new() { Dock = DockStyle.Top, Height = 215 };

        AddLabel(top, "Emri", 12, 18); txtPatientName.SetBounds(120, 15, 220, 28);
        AddLabel(top, "Telefoni", 12, 54); txtPatientPhone.SetBounds(120, 51, 220, 28);
        AddLabel(top, "Adresa", 12, 90); txtPatientAddress.SetBounds(120, 87, 220, 28);
        AddLabel(top, "Datelindja", 12, 126); dtpPatientDob.SetBounds(120, 123, 220, 28);
        AddLabel(top, "Kerko", 390, 18); txtPatientSearch.SetBounds(450, 15, 220, 28);

        Button btnSearch = CreateButton("Search", 680, 15, (_, _) => LoadPatients(txtPatientSearch.Text.Trim()));
        Button btnAdd = CreateButton("Add", 390, 70, (_, _) => AddPatient());
        Button btnUpdate = CreateButton("Update", 500, 70, (_, _) => UpdatePatient());
        Button btnDelete = CreateButton("Delete", 610, 70, (_, _) => DeletePatient());
        Button btnClear = CreateButton("Clear", 720, 70, (_, _) => ClearPatientInputs());

        top.Controls.AddRange([txtPatientName, txtPatientPhone, txtPatientAddress, dtpPatientDob, txtPatientSearch, btnSearch, btnAdd, btnUpdate, btnDelete, btnClear]);

        dgvPatients.SelectionChanged += (_, _) => FillPatientInputsFromGrid();
        tab.Controls.Add(dgvPatients);
        tab.Controls.Add(top);
        return tab;
    }

    private TabPage CreateDoctorsTab()
    {
        TabPage tab = new("Doctors");
        Panel top = new() { Dock = DockStyle.Top, Height = 215 };

        AddLabel(top, "Emri", 12, 18); txtDoctorName.SetBounds(120, 15, 220, 28);
        AddLabel(top, "Specializimi", 12, 54); txtSpecialization.SetBounds(120, 51, 220, 28);
        AddLabel(top, "Telefoni", 12, 90); txtDoctorPhone.SetBounds(120, 87, 220, 28);
        AddLabel(top, "Email", 390, 18); txtDoctorEmail.SetBounds(500, 15, 220, 28);
        AddLabel(top, "Orari", 390, 54); txtDoctorSchedule.SetBounds(500, 51, 220, 28);

        Button btnAdd = CreateButton("Add", 390, 95, (_, _) => AddDoctor());
        Button btnUpdate = CreateButton("Update", 500, 95, (_, _) => UpdateDoctor());
        Button btnDelete = CreateButton("Delete", 610, 95, (_, _) => DeleteDoctor());
        Button btnClear = CreateButton("Clear", 720, 95, (_, _) => ClearDoctorInputs());

        top.Controls.AddRange([txtDoctorName, txtSpecialization, txtDoctorPhone, txtDoctorEmail, txtDoctorSchedule, btnAdd, btnUpdate, btnDelete, btnClear]);

        dgvDoctors.SelectionChanged += (_, _) => FillDoctorInputsFromGrid();
        tab.Controls.Add(dgvDoctors);
        tab.Controls.Add(top);
        return tab;
    }

    private TabPage CreateAppointmentsTab()
    {
        TabPage tab = new("Appointments");
        Panel top = new() { Dock = DockStyle.Top, Height = 215 };

        AddLabel(top, "Pacienti", 12, 18); cmbAppointmentPatient.SetBounds(120, 15, 220, 28);
        AddLabel(top, "Doktori", 12, 54); cmbAppointmentDoctor.SetBounds(120, 51, 220, 28);
        AddLabel(top, "Data/Ora", 390, 18); dtpAppointment.SetBounds(500, 15, 220, 28);
        AddLabel(top, "Statusi", 390, 54); cmbAppointmentStatus.SetBounds(500, 51, 220, 28);
        cmbAppointmentStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbAppointmentStatus.Items.AddRange(new[] { "Pending", "Completed", "Cancelled" });

        Button btnAdd = CreateButton("Add", 390, 95, (_, _) => AddAppointment());
        Button btnUpdate = CreateButton("Update", 500, 95, (_, _) => UpdateAppointment());
        Button btnDelete = CreateButton("Delete", 610, 95, (_, _) => DeleteAppointment());

        top.Controls.AddRange([cmbAppointmentPatient, cmbAppointmentDoctor, dtpAppointment, cmbAppointmentStatus, btnAdd, btnUpdate, btnDelete]);

        dgvAppointments.SelectionChanged += (_, _) => FillAppointmentInputsFromGrid();
        tab.Controls.Add(dgvAppointments);
        tab.Controls.Add(top);
        return tab;
    }

    private TabPage CreateRecordsTab()
    {
        TabPage tab = new("Medical Records");
        Panel top = new() { Dock = DockStyle.Top, Height = 240 };

        AddLabel(top, "Appointment", 12, 18); cmbRecordAppointment.SetBounds(120, 15, 300, 28);
        AddLabel(top, "Diagnoza", 12, 54); txtDiagnosis.SetBounds(120, 51, 300, 28);
        AddLabel(top, "Simptomat", 12, 90); txtSymptoms.SetBounds(120, 87, 300, 28);
        AddLabel(top, "Ilacet", 450, 18); txtMedicines.SetBounds(540, 15, 300, 28);
        AddLabel(top, "Shenime", 450, 54); txtDoctorNotes.SetBounds(540, 51, 300, 64); txtDoctorNotes.Multiline = true;

        Button btnAdd = CreateButton("Add", 450, 130, (_, _) => AddRecord());
        Button btnUpdate = CreateButton("Update", 560, 130, (_, _) => UpdateRecord());
        Button btnDelete = CreateButton("Delete", 670, 130, (_, _) => DeleteRecord());

        top.Controls.AddRange([cmbRecordAppointment, txtDiagnosis, txtSymptoms, txtMedicines, txtDoctorNotes, btnAdd, btnUpdate, btnDelete]);

        dgvRecords.SelectionChanged += (_, _) => FillRecordInputsFromGrid();
        tab.Controls.Add(dgvRecords);
        tab.Controls.Add(top);
        return tab;
    }

    private TabPage CreateBillingTab()
    {
        TabPage tab = new("Billing");
        Panel top = new() { Dock = DockStyle.Top, Height = 215 };

        AddLabel(top, "Pacienti", 12, 18); cmbBillPatient.SetBounds(120, 15, 220, 28);
        AddLabel(top, "Sherbimi", 12, 54); txtBillService.SetBounds(120, 51, 220, 28);
        AddLabel(top, "Cmimi", 390, 18); txtBillPrice.SetBounds(500, 15, 220, 28);
        AddLabel(top, "Statusi", 390, 54); cmbBillStatus.SetBounds(500, 51, 220, 28);
        cmbBillStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbBillStatus.Items.AddRange(new[] { "Paid", "Unpaid" });

        Button btnAdd = CreateButton("Add", 390, 95, (_, _) => AddBill());
        Button btnUpdate = CreateButton("Update", 500, 95, (_, _) => UpdateBill());
        Button btnDelete = CreateButton("Delete", 610, 95, (_, _) => DeleteBill());

        top.Controls.AddRange([cmbBillPatient, txtBillService, txtBillPrice, cmbBillStatus, btnAdd, btnUpdate, btnDelete]);

        dgvBills.SelectionChanged += (_, _) => FillBillInputsFromGrid();
        tab.Controls.Add(dgvBills);
        tab.Controls.Add(top);
        return tab;
    }

    private TabPage CreateRoomsTab()
    {
        TabPage tab = new("Rooms");
        Panel top = new() { Dock = DockStyle.Top, Height = 215 };

        AddLabel(top, "Nr Dhome", 12, 18); txtRoomNumber.SetBounds(120, 15, 220, 28);
        AddLabel(top, "Tipi", 12, 54); txtRoomType.SetBounds(120, 51, 220, 28);
        AddLabel(top, "Statusi", 390, 18); cmbRoomStatus.SetBounds(500, 15, 220, 28);
        cmbRoomStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRoomStatus.Items.AddRange(new[] { "Available", "Occupied" });

        Button btnAdd = CreateButton("Add", 390, 95, (_, _) => AddRoom());
        Button btnUpdate = CreateButton("Update", 500, 95, (_, _) => UpdateRoom());
        Button btnDelete = CreateButton("Delete", 610, 95, (_, _) => DeleteRoom());

        top.Controls.AddRange([txtRoomNumber, txtRoomType, cmbRoomStatus, btnAdd, btnUpdate, btnDelete]);

        dgvRooms.SelectionChanged += (_, _) => FillRoomInputsFromGrid();
        tab.Controls.Add(dgvRooms);
        tab.Controls.Add(top);
        return tab;
    }

    private static void AddLabel(Control parent, string text, int x, int y)
    {
        parent.Controls.Add(new Label { Text = text, AutoSize = true, Location = new Point(x, y + 5) });
    }

    private static Button CreateButton(string text, int x, int y, EventHandler onClick)
    {
        Button button = new() { Text = text, Location = new Point(x, y), Width = 100, Height = 32 };
        button.Click += onClick;
        return button;
    }

    private void LoadData()
    {
        RefreshDashboard();
        LoadPatients();
        LoadDoctors();
        LoadAppointments();
        LoadRecords();
        LoadBills();
        LoadRooms();
        LoadCombos();
    }

    private void RefreshDashboard()
    {
        lblPatients.Text = DbManager.ExecuteScalar("SELECT COUNT(*) FROM Patients")?.ToString() ?? "0";
        lblDoctors.Text = DbManager.ExecuteScalar("SELECT COUNT(*) FROM Doctors")?.ToString() ?? "0";
        lblTodayAppointments.Text = DbManager.ExecuteScalar("SELECT COUNT(*) FROM Appointments WHERE CAST(AppointmentDateTime AS DATE) = CAST(GETDATE() AS DATE)")?.ToString() ?? "0";
        lblUnpaidBills.Text = DbManager.ExecuteScalar("SELECT COUNT(*) FROM Bills WHERE Status = 'Unpaid'")?.ToString() ?? "0";
    }

    private void LoadCombos()
    {
        DataTable patients = DbManager.ExecuteQuery("SELECT PatientId, FullName FROM Patients ORDER BY FullName");
        BindCombo(cmbAppointmentPatient, patients, "FullName", "PatientId");
        BindCombo(cmbBillPatient, patients.Copy(), "FullName", "PatientId");

        DataTable doctors = DbManager.ExecuteQuery("SELECT DoctorId, FullName FROM Doctors ORDER BY FullName");
        BindCombo(cmbAppointmentDoctor, doctors, "FullName", "DoctorId");

        DataTable appts = DbManager.ExecuteQuery(@"SELECT a.AppointmentId, CONCAT(p.FullName, ' - ', d.FullName, ' - ', FORMAT(a.AppointmentDateTime,'dd/MM/yyyy HH:mm')) AS Name
                                                  FROM Appointments a
                                                  INNER JOIN Patients p ON p.PatientId = a.PatientId
                                                  INNER JOIN Doctors d ON d.DoctorId = a.DoctorId
                                                  ORDER BY a.AppointmentDateTime DESC");
        BindCombo(cmbRecordAppointment, appts, "Name", "AppointmentId");
    }

    private static void BindCombo(ComboBox combo, DataTable table, string display, string value)
    {
        combo.DataSource = table;
        combo.DisplayMember = display;
        combo.ValueMember = value;
        combo.SelectedIndex = table.Rows.Count > 0 ? 0 : -1;
    }

    private void LoadPatients(string search = "")
    {
        string query = "SELECT PatientId, FullName, DateOfBirth, Phone, Address FROM Patients";
        if (!string.IsNullOrWhiteSpace(search))
        {
            query += " WHERE FullName LIKE @Search OR Phone LIKE @Search";
            dgvPatients.DataSource = DbManager.ExecuteQuery(query, new SqlParameter("@Search", $"%{search}%"));
            return;
        }

        dgvPatients.DataSource = DbManager.ExecuteQuery(query + " ORDER BY PatientId DESC");
    }

    private void AddPatient()
    {
        if (string.IsNullOrWhiteSpace(txtPatientName.Text) || string.IsNullOrWhiteSpace(txtPatientPhone.Text))
        {
            MessageBox.Show("Emri dhe telefoni jane te detyrueshem.");
            return;
        }

        DbManager.ExecuteNonQuery("INSERT INTO Patients (FullName, DateOfBirth, Phone, Address) VALUES (@Name,@Dob,@Phone,@Address)",
            new SqlParameter("@Name", txtPatientName.Text.Trim()),
            new SqlParameter("@Dob", dtpPatientDob.Value.Date),
            new SqlParameter("@Phone", txtPatientPhone.Text.Trim()),
            new SqlParameter("@Address", txtPatientAddress.Text.Trim()));

        MessageBox.Show("Pacienti u shtua me sukses.");
        LoadPatients(); LoadCombos(); RefreshDashboard();
        ClearPatientInputs();
    }

    private void UpdatePatient()
    {
        if (!TryGetSelectedId(dgvPatients, "PatientId", out int id)) return;

        DbManager.ExecuteNonQuery("UPDATE Patients SET FullName=@Name, DateOfBirth=@Dob, Phone=@Phone, Address=@Address WHERE PatientId=@Id",
            new SqlParameter("@Name", txtPatientName.Text.Trim()),
            new SqlParameter("@Dob", dtpPatientDob.Value.Date),
            new SqlParameter("@Phone", txtPatientPhone.Text.Trim()),
            new SqlParameter("@Address", txtPatientAddress.Text.Trim()),
            new SqlParameter("@Id", id));

        MessageBox.Show("Pacienti u perditesua.");
        LoadPatients(); LoadCombos();
    }

    private void DeletePatient()
    {
        if (!TryGetSelectedId(dgvPatients, "PatientId", out int id)) return;

        DbManager.ExecuteNonQuery("DELETE FROM Patients WHERE PatientId=@Id", new SqlParameter("@Id", id));
        MessageBox.Show("Pacienti u fshi.");
        LoadPatients(); LoadCombos(); RefreshDashboard();
    }

    private void FillPatientInputsFromGrid()
    {
        if (dgvPatients.CurrentRow?.Cells["PatientId"].Value is null) return;
        txtPatientName.Text = dgvPatients.CurrentRow.Cells["FullName"].Value?.ToString();
        txtPatientPhone.Text = dgvPatients.CurrentRow.Cells["Phone"].Value?.ToString();
        txtPatientAddress.Text = dgvPatients.CurrentRow.Cells["Address"].Value?.ToString();
        if (DateTime.TryParse(dgvPatients.CurrentRow.Cells["DateOfBirth"].Value?.ToString(), out DateTime dob)) dtpPatientDob.Value = dob;
    }

    private void ClearPatientInputs() => (txtPatientName.Text, txtPatientPhone.Text, txtPatientAddress.Text) = ("", "", "");

    private void LoadDoctors() => dgvDoctors.DataSource = DbManager.ExecuteQuery("SELECT DoctorId, FullName, Specialization, Phone, Email, WorkSchedule FROM Doctors ORDER BY DoctorId DESC");

    private void AddDoctor()
    {
        if (string.IsNullOrWhiteSpace(txtDoctorName.Text) || string.IsNullOrWhiteSpace(txtSpecialization.Text))
        {
            MessageBox.Show("Emri dhe specializimi jane te detyrueshem.");
            return;
        }

        DbManager.ExecuteNonQuery("INSERT INTO Doctors (FullName, Specialization, Phone, Email, WorkSchedule) VALUES (@Name,@Spec,@Phone,@Email,@Schedule)",
            new SqlParameter("@Name", txtDoctorName.Text.Trim()),
            new SqlParameter("@Spec", txtSpecialization.Text.Trim()),
            new SqlParameter("@Phone", txtDoctorPhone.Text.Trim()),
            new SqlParameter("@Email", txtDoctorEmail.Text.Trim()),
            new SqlParameter("@Schedule", txtDoctorSchedule.Text.Trim()));

        MessageBox.Show("Doktori u shtua.");
        LoadDoctors(); LoadCombos(); RefreshDashboard();
        ClearDoctorInputs();
    }

    private void UpdateDoctor()
    {
        if (!TryGetSelectedId(dgvDoctors, "DoctorId", out int id)) return;

        DbManager.ExecuteNonQuery("UPDATE Doctors SET FullName=@Name, Specialization=@Spec, Phone=@Phone, Email=@Email, WorkSchedule=@Schedule WHERE DoctorId=@Id",
            new SqlParameter("@Name", txtDoctorName.Text.Trim()),
            new SqlParameter("@Spec", txtSpecialization.Text.Trim()),
            new SqlParameter("@Phone", txtDoctorPhone.Text.Trim()),
            new SqlParameter("@Email", txtDoctorEmail.Text.Trim()),
            new SqlParameter("@Schedule", txtDoctorSchedule.Text.Trim()),
            new SqlParameter("@Id", id));

        MessageBox.Show("Doktori u perditesua.");
        LoadDoctors(); LoadCombos();
    }

    private void DeleteDoctor()
    {
        if (!TryGetSelectedId(dgvDoctors, "DoctorId", out int id)) return;

        DbManager.ExecuteNonQuery("DELETE FROM Doctors WHERE DoctorId=@Id", new SqlParameter("@Id", id));
        MessageBox.Show("Doktori u fshi.");
        LoadDoctors(); LoadCombos(); RefreshDashboard();
    }

    private void FillDoctorInputsFromGrid()
    {
        if (dgvDoctors.CurrentRow?.Cells["DoctorId"].Value is null) return;
        txtDoctorName.Text = dgvDoctors.CurrentRow.Cells["FullName"].Value?.ToString();
        txtSpecialization.Text = dgvDoctors.CurrentRow.Cells["Specialization"].Value?.ToString();
        txtDoctorPhone.Text = dgvDoctors.CurrentRow.Cells["Phone"].Value?.ToString();
        txtDoctorEmail.Text = dgvDoctors.CurrentRow.Cells["Email"].Value?.ToString();
        txtDoctorSchedule.Text = dgvDoctors.CurrentRow.Cells["WorkSchedule"].Value?.ToString();
    }

    private void ClearDoctorInputs() => (txtDoctorName.Text, txtSpecialization.Text, txtDoctorPhone.Text, txtDoctorEmail.Text, txtDoctorSchedule.Text) = ("", "", "", "", "");

    private void LoadAppointments()
    {
        dgvAppointments.DataSource = DbManager.ExecuteQuery(@"SELECT a.AppointmentId, p.FullName AS Patient, d.FullName AS Doctor, a.AppointmentDateTime, a.Status
                                                              FROM Appointments a
                                                              INNER JOIN Patients p ON p.PatientId = a.PatientId
                                                              INNER JOIN Doctors d ON d.DoctorId = a.DoctorId
                                                              ORDER BY a.AppointmentDateTime DESC");
    }

    private void AddAppointment()
    {
        if (cmbAppointmentPatient.SelectedValue is null || cmbAppointmentDoctor.SelectedValue is null || cmbAppointmentStatus.SelectedItem is null)
        {
            MessageBox.Show("Plotesoni te gjitha fushat e terminit.");
            return;
        }

        DbManager.ExecuteNonQuery("INSERT INTO Appointments (PatientId, DoctorId, AppointmentDateTime, Status) VALUES (@PatientId,@DoctorId,@DateTime,@Status)",
            new SqlParameter("@PatientId", cmbAppointmentPatient.SelectedValue),
            new SqlParameter("@DoctorId", cmbAppointmentDoctor.SelectedValue),
            new SqlParameter("@DateTime", dtpAppointment.Value),
            new SqlParameter("@Status", cmbAppointmentStatus.SelectedItem.ToString()!));

        MessageBox.Show("Termini u krijua.");
        LoadAppointments(); LoadCombos(); RefreshDashboard();
    }

    private void UpdateAppointment()
    {
        if (!TryGetSelectedId(dgvAppointments, "AppointmentId", out int id) || cmbAppointmentStatus.SelectedItem is null) return;

        DbManager.ExecuteNonQuery("UPDATE Appointments SET PatientId=@PatientId, DoctorId=@DoctorId, AppointmentDateTime=@DateTime, Status=@Status WHERE AppointmentId=@Id",
            new SqlParameter("@PatientId", cmbAppointmentPatient.SelectedValue),
            new SqlParameter("@DoctorId", cmbAppointmentDoctor.SelectedValue),
            new SqlParameter("@DateTime", dtpAppointment.Value),
            new SqlParameter("@Status", cmbAppointmentStatus.SelectedItem.ToString()!),
            new SqlParameter("@Id", id));

        MessageBox.Show("Termini u perditesua.");
        LoadAppointments(); LoadCombos(); RefreshDashboard();
    }

    private void DeleteAppointment()
    {
        if (!TryGetSelectedId(dgvAppointments, "AppointmentId", out int id)) return;
        DbManager.ExecuteNonQuery("DELETE FROM Appointments WHERE AppointmentId=@Id", new SqlParameter("@Id", id));
        MessageBox.Show("Termini u fshi.");
        LoadAppointments(); LoadCombos(); RefreshDashboard();
    }

    private void FillAppointmentInputsFromGrid()
    {
        if (dgvAppointments.CurrentRow?.Cells["AppointmentId"].Value is null) return;
        cmbAppointmentPatient.Text = dgvAppointments.CurrentRow.Cells["Patient"].Value?.ToString();
        cmbAppointmentDoctor.Text = dgvAppointments.CurrentRow.Cells["Doctor"].Value?.ToString();
        cmbAppointmentStatus.Text = dgvAppointments.CurrentRow.Cells["Status"].Value?.ToString();
        if (DateTime.TryParse(dgvAppointments.CurrentRow.Cells["AppointmentDateTime"].Value?.ToString(), out DateTime dt)) dtpAppointment.Value = dt;
    }

    private void LoadRecords()
    {
        dgvRecords.DataSource = DbManager.ExecuteQuery(@"SELECT r.RecordId, r.AppointmentId, r.Diagnosis, r.Symptoms, r.Medicines, r.DoctorNotes
                                                         FROM MedicalRecords r
                                                         ORDER BY r.RecordId DESC");
    }

    private void AddRecord()
    {
        if (cmbRecordAppointment.SelectedValue is null || string.IsNullOrWhiteSpace(txtDiagnosis.Text))
        {
            MessageBox.Show("Appointment dhe diagnoza jane te detyrueshme.");
            return;
        }

        DbManager.ExecuteNonQuery("INSERT INTO MedicalRecords (AppointmentId, Diagnosis, Symptoms, Medicines, DoctorNotes) VALUES (@AppointmentId,@Diagnosis,@Symptoms,@Medicines,@DoctorNotes)",
            new SqlParameter("@AppointmentId", cmbRecordAppointment.SelectedValue),
            new SqlParameter("@Diagnosis", txtDiagnosis.Text.Trim()),
            new SqlParameter("@Symptoms", txtSymptoms.Text.Trim()),
            new SqlParameter("@Medicines", txtMedicines.Text.Trim()),
            new SqlParameter("@DoctorNotes", txtDoctorNotes.Text.Trim()));

        MessageBox.Show("Kartela mjekesore u shtua.");
        LoadRecords();
    }

    private void UpdateRecord()
    {
        if (!TryGetSelectedId(dgvRecords, "RecordId", out int id)) return;

        DbManager.ExecuteNonQuery("UPDATE MedicalRecords SET AppointmentId=@AppointmentId, Diagnosis=@Diagnosis, Symptoms=@Symptoms, Medicines=@Medicines, DoctorNotes=@DoctorNotes WHERE RecordId=@Id",
            new SqlParameter("@AppointmentId", cmbRecordAppointment.SelectedValue),
            new SqlParameter("@Diagnosis", txtDiagnosis.Text.Trim()),
            new SqlParameter("@Symptoms", txtSymptoms.Text.Trim()),
            new SqlParameter("@Medicines", txtMedicines.Text.Trim()),
            new SqlParameter("@DoctorNotes", txtDoctorNotes.Text.Trim()),
            new SqlParameter("@Id", id));

        MessageBox.Show("Kartela u perditesua.");
        LoadRecords();
    }

    private void DeleteRecord()
    {
        if (!TryGetSelectedId(dgvRecords, "RecordId", out int id)) return;
        DbManager.ExecuteNonQuery("DELETE FROM MedicalRecords WHERE RecordId=@Id", new SqlParameter("@Id", id));
        MessageBox.Show("Kartela u fshi.");
        LoadRecords();
    }

    private void FillRecordInputsFromGrid()
    {
        if (dgvRecords.CurrentRow?.Cells["RecordId"].Value is null) return;
        cmbRecordAppointment.SelectedValue = dgvRecords.CurrentRow.Cells["AppointmentId"].Value;
        txtDiagnosis.Text = dgvRecords.CurrentRow.Cells["Diagnosis"].Value?.ToString();
        txtSymptoms.Text = dgvRecords.CurrentRow.Cells["Symptoms"].Value?.ToString();
        txtMedicines.Text = dgvRecords.CurrentRow.Cells["Medicines"].Value?.ToString();
        txtDoctorNotes.Text = dgvRecords.CurrentRow.Cells["DoctorNotes"].Value?.ToString();
    }

    private void LoadBills()
    {
        dgvBills.DataSource = DbManager.ExecuteQuery(@"SELECT b.BillId, p.FullName AS Patient, b.ServiceName, b.Price, b.Status, b.CreatedAt
                                                       FROM Bills b
                                                       INNER JOIN Patients p ON p.PatientId = b.PatientId
                                                       ORDER BY b.BillId DESC");
    }

    private void AddBill()
    {
        if (cmbBillPatient.SelectedValue is null || string.IsNullOrWhiteSpace(txtBillService.Text) || !decimal.TryParse(txtBillPrice.Text, out decimal price) || cmbBillStatus.SelectedItem is null)
        {
            MessageBox.Show("Kontrolloni inputin e fatures.");
            return;
        }

        DbManager.ExecuteNonQuery("INSERT INTO Bills (PatientId, ServiceName, Price, Status) VALUES (@PatientId,@ServiceName,@Price,@Status)",
            new SqlParameter("@PatientId", cmbBillPatient.SelectedValue),
            new SqlParameter("@ServiceName", txtBillService.Text.Trim()),
            new SqlParameter("@Price", price),
            new SqlParameter("@Status", cmbBillStatus.SelectedItem.ToString()!));

        MessageBox.Show("Fatura u krijua.");
        LoadBills(); RefreshDashboard();
    }

    private void UpdateBill()
    {
        if (!TryGetSelectedId(dgvBills, "BillId", out int id) || !decimal.TryParse(txtBillPrice.Text, out decimal price) || cmbBillStatus.SelectedItem is null) return;

        DbManager.ExecuteNonQuery("UPDATE Bills SET PatientId=@PatientId, ServiceName=@ServiceName, Price=@Price, Status=@Status WHERE BillId=@Id",
            new SqlParameter("@PatientId", cmbBillPatient.SelectedValue),
            new SqlParameter("@ServiceName", txtBillService.Text.Trim()),
            new SqlParameter("@Price", price),
            new SqlParameter("@Status", cmbBillStatus.SelectedItem.ToString()!),
            new SqlParameter("@Id", id));

        MessageBox.Show("Fatura u perditesua.");
        LoadBills(); RefreshDashboard();
    }

    private void DeleteBill()
    {
        if (!TryGetSelectedId(dgvBills, "BillId", out int id)) return;
        DbManager.ExecuteNonQuery("DELETE FROM Bills WHERE BillId=@Id", new SqlParameter("@Id", id));
        MessageBox.Show("Fatura u fshi.");
        LoadBills(); RefreshDashboard();
    }

    private void FillBillInputsFromGrid()
    {
        if (dgvBills.CurrentRow?.Cells["BillId"].Value is null) return;
        cmbBillPatient.Text = dgvBills.CurrentRow.Cells["Patient"].Value?.ToString();
        txtBillService.Text = dgvBills.CurrentRow.Cells["ServiceName"].Value?.ToString();
        txtBillPrice.Text = dgvBills.CurrentRow.Cells["Price"].Value?.ToString();
        cmbBillStatus.Text = dgvBills.CurrentRow.Cells["Status"].Value?.ToString();
    }

    private void LoadRooms() => dgvRooms.DataSource = DbManager.ExecuteQuery("SELECT RoomId, RoomNumber, RoomType, Status FROM Rooms ORDER BY RoomId DESC");

    private void AddRoom()
    {
        if (string.IsNullOrWhiteSpace(txtRoomNumber.Text) || string.IsNullOrWhiteSpace(txtRoomType.Text) || cmbRoomStatus.SelectedItem is null)
        {
            MessageBox.Show("Plotesoni te dhenat e dhomes.");
            return;
        }

        DbManager.ExecuteNonQuery("INSERT INTO Rooms (RoomNumber, RoomType, Status) VALUES (@RoomNumber,@RoomType,@Status)",
            new SqlParameter("@RoomNumber", txtRoomNumber.Text.Trim()),
            new SqlParameter("@RoomType", txtRoomType.Text.Trim()),
            new SqlParameter("@Status", cmbRoomStatus.SelectedItem.ToString()!));

        MessageBox.Show("Dhoma u shtua.");
        LoadRooms();
    }

    private void UpdateRoom()
    {
        if (!TryGetSelectedId(dgvRooms, "RoomId", out int id) || cmbRoomStatus.SelectedItem is null) return;

        DbManager.ExecuteNonQuery("UPDATE Rooms SET RoomNumber=@RoomNumber, RoomType=@RoomType, Status=@Status WHERE RoomId=@Id",
            new SqlParameter("@RoomNumber", txtRoomNumber.Text.Trim()),
            new SqlParameter("@RoomType", txtRoomType.Text.Trim()),
            new SqlParameter("@Status", cmbRoomStatus.SelectedItem.ToString()!),
            new SqlParameter("@Id", id));

        MessageBox.Show("Dhoma u perditesua.");
        LoadRooms();
    }

    private void DeleteRoom()
    {
        if (!TryGetSelectedId(dgvRooms, "RoomId", out int id)) return;
        DbManager.ExecuteNonQuery("DELETE FROM Rooms WHERE RoomId=@Id", new SqlParameter("@Id", id));
        MessageBox.Show("Dhoma u fshi.");
        LoadRooms();
    }

    private void FillRoomInputsFromGrid()
    {
        if (dgvRooms.CurrentRow?.Cells["RoomId"].Value is null) return;
        txtRoomNumber.Text = dgvRooms.CurrentRow.Cells["RoomNumber"].Value?.ToString();
        txtRoomType.Text = dgvRooms.CurrentRow.Cells["RoomType"].Value?.ToString();
        cmbRoomStatus.Text = dgvRooms.CurrentRow.Cells["Status"].Value?.ToString();
    }

    private void ApplyRolePermissions()
    {
        if (role == "Receptionist")
        {
            if (tabControl.TabPages.Count > 4) tabControl.TabPages.RemoveAt(4);
        }
    }

    private static bool TryGetSelectedId(DataGridView grid, string column, out int id)
    {
        id = 0;
        if (grid.CurrentRow?.Cells[column].Value is null)
        {
            MessageBox.Show("Zgjidhni nje rresht nga tabela.");
            return false;
        }

        id = Convert.ToInt32(grid.CurrentRow.Cells[column].Value);
        return true;
    }
}
