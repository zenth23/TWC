using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWC.IMS.Common
{
    public static class Messages
    {
        public const int EVENTVIEWER_APP_EVENT_ID = 0;              // change this per project
        public const string EVENTVIEWER_APP_NAME = "BPS";   // change this per project

        public const string SQL_DELETE_STATEMENT_ERROR_MESSAGE = "The delete statement conflicted with the reference constraint.";
        public const string RECORD_IN_USE = "Record already in use. Cannot perform delete.";
        public const string RECORD_MODIFIED = "The record being updated has already been modified by another user. Please refresh your page to get the latest data.";
        public const string RECORD_DELETED = "The record being updated is already deleted by another user.";
        public const string RECORD_NOT_FOUND = "Record not found.";
        public const string NOT_AUTHORIZED = "You are not authorized to access this page. Please contact your system administrator.";
        public const string DO_NOT_REPLY = "This is a system-generated email. Do not reply.";
        public const string SESSION_EXPIRED = "Your session has expired. Please refresh the browser.";
        public const string MUST_LOGIN_FIRST = "You must login first.";
        public const string SOMETHING_WENT_WRONG = "Something went wrong. Please contact your system administrator.";
        public const string ACCOUNT_NOT_ACTIVE = "Your account is not active. Please contact your system administrator.";
        public const string ACCOUNT_LOCKED_OUT = "Your account has been locked out. Please contact your system administrator.";
        public const string ACCOUNT_EXPIRED = "Your account has expired. Please contact your system administrator.";
        public const string ACCOUNT_NO_DETAILS = "Unable to get details of your account. Please contact your system administrator.";
        public const string NONADMIN_ROLE = "Your role has no admin privileges.";
    }

}
