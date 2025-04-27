using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common.HelperClasses
{
    public enum StatusType
    {
        SUCCESS,
        FAILED,
        ERROR
    }

    public enum SftpConfigName
    {
        SFTP_HOST,
        SFTP_USERNAME,
        SFTP_PASSWORD,
        SFTP_PORT
    }

    public enum SystemConfigName
    {
        BACKUP_UPLOAD_ENC_DIR,
        DOWNLOAD_DEC_DIR,
        EMAIL_FROM,
        ENCRYPT_VALUE_CONFIGS,
        GPG_EXE_FILE_PATH,
        GPG_HOME_DIR,
        GPG_SAP_USERID_HR,
        GPG_SAP_USERID_SMPI,
        GPG_TMS_PASSPHRASE,
        GPG_TMS_USERID,
        SMTP_EMAILPASS,
        SMTP_ENABLESSL,
        SMTP_HOST,
        SMTP_MAILER_NAME,
        SMTP_PORT,
        SMS_API_ORIGINATOR,
        SMS_API_PASSWORD,
        SMS_API_URL,
        SMS_API_URL_VALIDATE,
        SMS_API_USERNAME
    }

    public enum TwoFactorAuthType
    {
        GOOGLE_ENABLED,
        GOOGLE_DISABLED,
        MICROSOFT_ENABLED,
        MICROSOFT_DISABLED,
        VERIFY_EMAIL
    }

    public enum TwoFactorAuthProvider
    {
        GOOGLE_AUTH,
        MICROSOFT_AUTH,
        SMS,
        EMAIL
    }
}
