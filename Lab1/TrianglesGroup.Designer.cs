namespace Lab1
{
    partial class TrianglesGroupUC
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupName = new System.Windows.Forms.Label();
            this.editButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // groupName
            // 
            this.groupName.Location = new System.Drawing.Point(3, 10);
            this.groupName.Name = "groupName";
            this.groupName.Size = new System.Drawing.Size(100, 23);
            this.groupName.TabIndex = 0;
            this.groupName.Text = "groupName";
            this.groupName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // editButton
            // 
            this.editButton.Location = new System.Drawing.Point(71, 10);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(100, 23);
            this.editButton.TabIndex = 1;
            this.editButton.Text = "Edit group";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // TrianglesGroupUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.groupName);
            this.Name = "TrianglesGroupUC";
            this.Size = new System.Drawing.Size(174, 42);
            this.Load += new System.EventHandler(this.TrianglesGroup_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label groupName;
        private System.Windows.Forms.Button editButton;
    }
}
