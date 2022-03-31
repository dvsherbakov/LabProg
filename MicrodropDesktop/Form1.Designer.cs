namespace MicrodropDesktop
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbLog = new System.Windows.Forms.ListBox();
            this.btnRequest = new System.Windows.Forms.Button();
            this.btnGreqency = new System.Windows.Forms.Button();
            this.btnStrobe = new System.Windows.Forms.Button();
            this.btnSetFreq = new System.Windows.Forms.Button();
            this.btnImpulseLength = new System.Windows.Forms.Button();
            this.cbPulseNo = new System.Windows.Forms.ComboBox();
            this.btSetImpulseLength = new System.Windows.Forms.Button();
            this.tbPulseValue = new System.Windows.Forms.TextBox();
            this.btnSetImpulseVoltage = new System.Windows.Forms.Button();
            this.btnSetPulseDelay = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.cbHead = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lbLog
            // 
            this.lbLog.FormattingEnabled = true;
            this.lbLog.ItemHeight = 25;
            this.lbLog.Location = new System.Drawing.Point(12, 226);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(813, 204);
            this.lbLog.TabIndex = 0;
            // 
            // btnRequest
            // 
            this.btnRequest.Location = new System.Drawing.Point(12, 12);
            this.btnRequest.Name = "btnRequest";
            this.btnRequest.Size = new System.Drawing.Size(112, 34);
            this.btnRequest.TabIndex = 1;
            this.btnRequest.Text = "Запрос";
            this.btnRequest.UseVisualStyleBackColor = true;
            this.btnRequest.Click += new System.EventHandler(this.BtnRequest_Click);
            // 
            // btnGreqency
            // 
            this.btnGreqency.Location = new System.Drawing.Point(12, 52);
            this.btnGreqency.Name = "btnGreqency";
            this.btnGreqency.Size = new System.Drawing.Size(112, 34);
            this.btnGreqency.TabIndex = 2;
            this.btnGreqency.Text = "Частота";
            this.btnGreqency.UseVisualStyleBackColor = true;
            this.btnGreqency.Click += new System.EventHandler(this.BtnFrequency_Click);
            // 
            // btnStrobe
            // 
            this.btnStrobe.Location = new System.Drawing.Point(12, 102);
            this.btnStrobe.Name = "btnStrobe";
            this.btnStrobe.Size = new System.Drawing.Size(112, 34);
            this.btnStrobe.TabIndex = 3;
            this.btnStrobe.Text = "Стробоскоб";
            this.btnStrobe.UseVisualStyleBackColor = true;
            this.btnStrobe.Click += new System.EventHandler(this.BtnStrobe_Click);
            // 
            // btnSetFreq
            // 
            this.btnSetFreq.Location = new System.Drawing.Point(151, 52);
            this.btnSetFreq.Name = "btnSetFreq";
            this.btnSetFreq.Size = new System.Drawing.Size(112, 34);
            this.btnSetFreq.TabIndex = 4;
            this.btnSetFreq.Text = "Уст частоту";
            this.btnSetFreq.UseVisualStyleBackColor = true;
            this.btnSetFreq.Click += new System.EventHandler(this.Button1_Click);
            // 
            // btnImpulseLength
            // 
            this.btnImpulseLength.Location = new System.Drawing.Point(12, 153);
            this.btnImpulseLength.Name = "btnImpulseLength";
            this.btnImpulseLength.Size = new System.Drawing.Size(112, 34);
            this.btnImpulseLength.TabIndex = 5;
            this.btnImpulseLength.Text = "Длинна имп";
            this.btnImpulseLength.UseVisualStyleBackColor = true;
            this.btnImpulseLength.Click += new System.EventHandler(this.Button1_Click_1);
            // 
            // cbPulseNo
            // 
            this.cbPulseNo.FormattingEnabled = true;
            this.cbPulseNo.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cbPulseNo.Location = new System.Drawing.Point(288, 13);
            this.cbPulseNo.Name = "cbPulseNo";
            this.cbPulseNo.Size = new System.Drawing.Size(52, 33);
            this.cbPulseNo.TabIndex = 6;
            // 
            // btSetImpulseLength
            // 
            this.btSetImpulseLength.Location = new System.Drawing.Point(454, 12);
            this.btSetImpulseLength.Name = "btSetImpulseLength";
            this.btSetImpulseLength.Size = new System.Drawing.Size(112, 34);
            this.btSetImpulseLength.TabIndex = 7;
            this.btSetImpulseLength.Text = "Уст дл имп";
            this.btSetImpulseLength.UseVisualStyleBackColor = true;
            this.btSetImpulseLength.Click += new System.EventHandler(this.BtSetImpulseLength_Click);
            // 
            // tbPulseValue
            // 
            this.tbPulseValue.Location = new System.Drawing.Point(346, 14);
            this.tbPulseValue.Name = "tbPulseValue";
            this.tbPulseValue.Size = new System.Drawing.Size(102, 31);
            this.tbPulseValue.TabIndex = 8;
            // 
            // btnSetImpulseVoltage
            // 
            this.btnSetImpulseVoltage.Location = new System.Drawing.Point(572, 12);
            this.btnSetImpulseVoltage.Name = "btnSetImpulseVoltage";
            this.btnSetImpulseVoltage.Size = new System.Drawing.Size(112, 34);
            this.btnSetImpulseVoltage.TabIndex = 9;
            this.btnSetImpulseVoltage.Text = "Уст напр";
            this.btnSetImpulseVoltage.UseVisualStyleBackColor = true;
            this.btnSetImpulseVoltage.Click += new System.EventHandler(this.BtnSetImpulseVoltage_Click);
            // 
            // btnSetPulseDelay
            // 
            this.btnSetPulseDelay.Location = new System.Drawing.Point(690, 12);
            this.btnSetPulseDelay.Name = "btnSetPulseDelay";
            this.btnSetPulseDelay.Size = new System.Drawing.Size(135, 34);
            this.btnSetPulseDelay.TabIndex = 10;
            this.btnSetPulseDelay.Text = "Уст задержку";
            this.btnSetPulseDelay.UseVisualStyleBackColor = true;
            this.btnSetPulseDelay.Click += new System.EventHandler(this.BtnSetPulseDelay_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(481, 175);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(112, 34);
            this.btnStart.TabIndex = 11;
            this.btnStart.Text = "Старт";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Location = new System.Drawing.Point(613, 175);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(112, 34);
            this.btnStop.TabIndex = 12;
            this.btnStop.Text = "Стоп";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // cbHead
            // 
            this.cbHead.FormattingEnabled = true;
            this.cbHead.Items.AddRange(new object[] {
            "1",
            "2"});
            this.cbHead.Location = new System.Drawing.Point(288, 102);
            this.cbHead.Name = "cbHead";
            this.cbHead.Size = new System.Drawing.Size(52, 33);
            this.cbHead.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 450);
            this.Controls.Add(this.cbHead);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnSetPulseDelay);
            this.Controls.Add(this.btnSetImpulseVoltage);
            this.Controls.Add(this.tbPulseValue);
            this.Controls.Add(this.btSetImpulseLength);
            this.Controls.Add(this.cbPulseNo);
            this.Controls.Add(this.btnImpulseLength);
            this.Controls.Add(this.btnSetFreq);
            this.Controls.Add(this.btnStrobe);
            this.Controls.Add(this.btnGreqency);
            this.Controls.Add(this.btnRequest);
            this.Controls.Add(this.lbLog);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Button btnRequest;
        private System.Windows.Forms.Button btnGreqency;
        private System.Windows.Forms.Button btnStrobe;
        private System.Windows.Forms.Button btnSetFreq;
        private System.Windows.Forms.Button btnImpulseLength;
        private System.Windows.Forms.ComboBox cbPulseNo;
        private System.Windows.Forms.Button btSetImpulseLength;
        private System.Windows.Forms.TextBox tbPulseValue;
        private System.Windows.Forms.Button btnSetImpulseVoltage;
        private System.Windows.Forms.Button btnSetPulseDelay;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ComboBox cbHead;
    }
}
