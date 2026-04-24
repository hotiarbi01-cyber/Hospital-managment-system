using System.Data;
using Microsoft.Data.SqlClient;
using HospitalManagementSystem.Data;

namespace HospitalManagementSystem.Forms;

public class LoginForm : Form
{
    private readonly ComboBox cmbRole = new();
    private readonly TextBox txtUsername = new();
    private readonly TextBox txtPassword = new();
    private readonly Button btnLogin = new();

    public LoginForm()
    {
        Text = "Hospital Management - Login";
        Width = 430;
        Height = 320;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        Label lblTitle = new()
        {
            Text = "Hospital Management System",
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Location = new Point(80, 20)
        };

        Label lblRole = new() { Text = "Role", AutoSize = true, Location = new Point(50, 80) };
        cmbRole.Location = new Point(150, 75);
        cmbRole.Width = 200;
        cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRole.Items.AddRange(new[] { "Admin", "Doctor", "Receptionist" });

        Label lblUsername = new() { Text = "Username", AutoSize = true, Location = new Point(50, 120) };
        txtUsername.Location = new Point(150, 115);
        txtUsername.Width = 200;

        Label lblPassword = new() { Text = "Password", AutoSize = true, Location = new Point(50, 160) };
        txtPassword.Location = new Point(150, 155);
        txtPassword.Width = 200;
        txtPassword.PasswordChar = '*';

        btnLogin.Text = "Login";
        btnLogin.Location = new Point(150, 210);
        btnLogin.Width = 120;
        btnLogin.Height = 35;
        btnLogin.Click += BtnLogin_Click;

        Controls.AddRange([lblTitle, lblRole, cmbRole, lblUsername, txtUsername, lblPassword, txtPassword, btnLogin]);
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        if (cmbRole.SelectedItem is null || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            MessageBox.Show("Ploteso te gjitha fushat.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string query = @"SELECT COUNT(*)
                         FROM Users
                         WHERE Username = @Username AND PasswordHash = @PasswordHash AND Role = @Role";

        object? result = DbManager.ExecuteScalar(
            query,
            new SqlParameter("@Username", txtUsername.Text.Trim()),
            new SqlParameter("@PasswordHash", txtPassword.Text.Trim()),
            new SqlParameter("@Role", cmbRole.SelectedItem.ToString()!)
        );

        int count = Convert.ToInt32(result);

        if (count > 0)
        {
            Hide();
            MainForm mainForm = new(cmbRole.SelectedItem.ToString()!, txtUsername.Text.Trim());
            mainForm.FormClosed += (_, _) => Close();
            mainForm.Show();
        }
        else
        {
            MessageBox.Show("Kredenciale te pasakta.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
