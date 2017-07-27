namespace Bridge
{
    partial class ManagerForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.ErrorMsgLabel = new System.Windows.Forms.Label();
            this.CHAI3D_data = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ErrorMsgLabel
            // 
            this.ErrorMsgLabel.Location = new System.Drawing.Point(164, 184);
            this.ErrorMsgLabel.Name = "ErrorMsgLabel";
            this.ErrorMsgLabel.Size = new System.Drawing.Size(200, 46);
            this.ErrorMsgLabel.TabIndex = 0;
            this.ErrorMsgLabel.Text = "error message";
            this.ErrorMsgLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CHAI3D_data
            // 
            this.CHAI3D_data.Location = new System.Drawing.Point(130, 76);
            this.CHAI3D_data.Name = "CHAI3D_data";
            this.CHAI3D_data.Size = new System.Drawing.Size(265, 23);
            this.CHAI3D_data.TabIndex = 1;
            this.CHAI3D_data.Text = "HIP";
            this.CHAI3D_data.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 308);
            this.Controls.Add(this.CHAI3D_data);
            this.Controls.Add(this.ErrorMsgLabel);
            this.Name = "ManagerForm";
            this.Text = "Bridge Program";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManagerForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label ErrorMsgLabel;
        private System.Windows.Forms.Label CHAI3D_data;
    }
}

