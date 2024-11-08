using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qlDiem
{
    public partial class MonHoc : Form
    {

        private string connString = "Data Source=.;Initial Catalog=qldiem1;Integrated Security=True";
        private DataTable dt;
        public MonHoc()
        {
            InitializeComponent();
            dt = new DataTable();
        }
        private DataTable GetCourseCatIDData(string query)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {
                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConn);
                adapter.Fill(dataTable);
            }
            return dataTable;
        }

        void loadCourseCatID()
        {
            string query = "SELECT * FROM CourseCategories";
            DataTable dtMajors = GetCourseCatIDData(query);
            cbCourseCatID.DataSource = dtMajors;
            cbCourseCatID.ValueMember = "CourseCatID";
            cbCourseCatID.DisplayMember = "CourseCatID";
        }

        private void LoadData(string searchCondition = "")
        {
            dataGridView1.AutoGenerateColumns = false;
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {

                sqlConn.Open();
                string sql = "SELECT * FROM Courses WHERE IsDeleted = 0" + searchCondition;
                SqlDataAdapter adapter = new SqlDataAdapter(sql, sqlConn);
                dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrEmpty(tbCourseCode.Text) || string.IsNullOrEmpty(tbCourseID.Text) || string.IsNullOrEmpty(tbCourseName.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection sqlConn = new SqlConnection(connString))
            {
                sqlConn.Open();
                string sql = $"INSERT INTO Courses (CourseID, CourseCode, CourseName, CourseCredits, ClassSessions, MaxAllowedAbsences, PointToPass, CourseCatID, IsDeleted) " +
                             $"VALUES (@ID, @Code, @Name, @Credits, @Sessions, @MaxAllowedAbsences, @PointToPass, @CourseCatID, 0)";
                SqlCommand cmd = new SqlCommand(sql, sqlConn);
                cmd.Parameters.AddWithValue("@ID", tbCourseID.Text);
                cmd.Parameters.AddWithValue("@Code", tbCourseCode.Text);
                cmd.Parameters.AddWithValue("@Name", tbCourseName.Text);
                cmd.Parameters.AddWithValue("@Credits", nrCourseCredit.Text);
                cmd.Parameters.AddWithValue("@Sessions", nrSessions.Text);
                cmd.Parameters.AddWithValue("@MaxAllowedAbsences", nrMAA.Text);
                cmd.Parameters.AddWithValue("@PointToPass", nrPTP.Text);
                cmd.Parameters.AddWithValue("@CourseCatID", cbCourseCatID.Text);
                cmd.ExecuteNonQuery();
                LoadData();
                MessageBox.Show("Thêm thành công!");
            }
        }

        private void MonHoc_Load(object sender, EventArgs e)
        {
            LoadData();
            loadCourseCatID();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (ValidateFields())
            {
                using (SqlConnection sqlConn = new SqlConnection(connString))
                {
                    sqlConn.Open();
                    string sql = $"UPDATE Courses SET CourseName = @Name, CourseCode = @Code, CourseCredits = @Credits, CourseCatID = @CourseCatID, " +
                                 $"ClassSessions = @ClassSessions, MaxAllowedAbsences = @MaxAllowedAbsences, PointToPass = @PointToPass " +
                                 $"WHERE CourseID = @CourseID";
                    SqlCommand cmd = new SqlCommand(sql, sqlConn);
                    cmd.Parameters.AddWithValue("@CourseID", tbCourseID.Text);
                    cmd.Parameters.AddWithValue("@Name", tbCourseName.Text);
                    cmd.Parameters.AddWithValue("@Code", tbCourseCode.Text);
                    cmd.Parameters.AddWithValue("@Credits", nrCourseCredit.Text);
                    cmd.Parameters.AddWithValue("@CourseCatID", cbCourseCatID.Text);
                    cmd.Parameters.AddWithValue("@ClassSessions", nrSessions.Text);
                    cmd.Parameters.AddWithValue("@MaxAllowedAbsences", nrMAA.Text);
                    cmd.Parameters.AddWithValue("@PointToPass", nrPTP.Text);
                    cmd.ExecuteNonQuery();
                    LoadData();
                    MessageBox.Show("Sửa thành công!");
                }
            }
        }
    }
}
