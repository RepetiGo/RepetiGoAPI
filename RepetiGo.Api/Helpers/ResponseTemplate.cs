namespace RepetiGo.Api.Helpers
{
    public static class ResponseTemplate
    {
        public static string GetEmailVerificationHtml(string safeLink)
        {
            return $@"<!DOCTYPE html>
<html>
<head>

  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Confirmation</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
  /**
   * Google webfonts. Recommended to include the .woff version for cross-client compatibility.
   */
  @media screen {{
    @font-face {{
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 400;
      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
    }}
    @font-face {{
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 700;
      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
    }}
  }}
  /**
   * Avoid browser level font resizing.
   * 1. Windows Mobile
   * 2. iOS / OSX
   */
  body,
  table,
  td,
  a {{
    -ms-text-size-adjust: 100%; /* 1 */
    -webkit-text-size-adjust: 100%; /* 2 */
  }}
  /**
   * Remove extra space added to tables and cells in Outlook.
   */
  table,
  td {{
    mso-table-rspace: 0pt;
    mso-table-lspace: 0pt;
  }}
  /**
   * Better fluid images in Internet Explorer.
   */
  img {{
    -ms-interpolation-mode: bicubic;
  }}
  /**
   * Remove blue links for iOS devices.
   */
  a[x-apple-data-detectors] {{
    font-family: inherit !important;
    font-size: inherit !important;
    font-weight: inherit !important;
    line-height: inherit !important;
    color: inherit !important;
    text-decoration: none !important;
  }}
  /**
   * Fix centering issues in Android 4.4.
   */
  div[style*=""margin: 16px 0;""] {{
    margin: 0 !important;
  }}
  body {{
    width: 100% !important;
    height: 100% !important;
    padding: 0 !important;
    margin: 0 !important;
  }}
  /**
   * Collapse table borders to avoid space between cells.
   */
  table {{
    border-collapse: collapse !important;
  }}
  a {{
    color: #1a82e2;
  }}
  img {{
    height: auto;
    line-height: 100%;
    text-decoration: none;
    border: 0;
    outline: none;
  }}
  </style>

</head>
<body style=""background-color: #e9ecef;"">

  <!-- start preheader -->
  <div class=""preheader"" style=""display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;"">
    A preheader is the short summary text that follows the subject line when an email is viewed in the inbox.
  </div>
  <!-- end preheader -->

  <!-- start body -->
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">

    <!-- start logo -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href=""https://zorlen.com"" target=""_blank"" style=""display: inline-block;"">
                <img src=""https://zorlen.com/favicon.ico"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end logo -->

    <!-- start hero -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf; text-align: center;"">
  <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;"">Email verification</h1>
</td>

          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end hero -->

    <!-- start copy block -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">Tap the button below to confirm your email address. If you didn't create an account with <b>RepetiGo</b>, you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start button -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"">
              <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                <tr>
                  <td align=""center"" bgcolor=""#ffffff"" style=""padding: 12px;"">
                    <table border=""0"" cellpadding=""0"" cellspacing=""0"">
                      <tr>
                        <td align=""center"" bgcolor=""#1a82e2"" style=""border-radius: 6px;"">
                          <a href=""{safeLink}"" target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">Click to verify</a>
                        </td>
                      </tr>
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
          <!-- end button -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">If that doesn't work, copy and paste the following link in your browser:</p>
              <p style=""margin: 0;""><a href=""{safeLink}"" target=""_blank"">{safeLink}</a></p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Cheers,<br> The RepetiGo Team</p>
            </td>
          </tr>
          <!-- end copy -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end copy block -->

    <!-- start footer -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px;"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start permission -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">You received this email because we received a request for email verification for your account. If you didn't request email verification you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end permission -->

          <!-- start unsubscribe -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">Ho Chi Minh City, Vietnam</p>
            </td>
          </tr>
          <!-- end unsubscribe -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end footer -->

  </table>
  <!-- end body -->

</body>
</html>";
        }

        public static string NotificationSuccessHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Verified Successfully</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
    @media screen {
      @font-face {
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 400;
        src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
      }
      @font-face {
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 700;
        src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
      }
    }
    body, table, td, a {
      -ms-text-size-adjust: 100%;
      -webkit-text-size-adjust: 100%;
    }
    table, td {
      mso-table-rspace: 0pt;
      mso-table-lspace: 0pt;
    }
    img {
      -ms-interpolation-mode: bicubic;
    }
    a[x-apple-data-detectors] {
      font-family: inherit !important;
      font-size: inherit !important;
      font-weight: inherit !important;
      line-height: inherit !important;
      color: inherit !important;
      text-decoration: none !important;
    }
    div[style*=""margin: 16px 0;""] {
      margin: 0 !important;
    }
    body {
      width: 100% !important;
      height: 100% !important;
      padding: 0 !important;
      margin: 0 !important;
      background-color: #e9ecef;
    }
    table {
      border-collapse: collapse !important;
    }
    a {
      color: #1a82e2;
    }
    img {
      height: auto;
      line-height: 100%;
      text-decoration: none;
      border: 0;
      outline: none;
    }
  </style>
</head>
<body style=""background-color: #e9ecef;"">
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href=""https://zorlen.com"" target=""_blank"" style=""display: inline-block;"">
                <img src=""https://zorlen.com/favicon.ico"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #28a745;"">
              <h1 style=""margin: 0; font-size: 32px; font-weight: 700; color: #28a745; letter-spacing: -1px; line-height: 48px; text-align: center;"">
  Email Verified!
</h1>
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">Your email address has been successfully verified. You can now log in and start using all features of your account.</p>
            </td>
          </tr>
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Thank you for confirming your email.<br>— The RepetiGo Team</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px;"">
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">If you did not request this verification, you can safely ignore this page.</p>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">Ho Chi Minh City, Vietnam</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";
        }

        public static string ResendVerificationEmailHtml(string safeLink)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Confirmation</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
    @media screen {{
      @font-face {{
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 400;
        src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
      }}
      @font-face {{
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 700;
        src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
      }}
    }}
    body, table, td, a {{
      -ms-text-size-adjust: 100%;
      -webkit-text-size-adjust: 100%;
    }}
    table, td {{
      mso-table-rspace: 0pt;
      mso-table-lspace: 0pt;
    }}
    img {{
      -ms-interpolation-mode: bicubic;
    }}
    a[x-apple-data-detectors] {{
      font-family: inherit !important;
      font-size: inherit !important;
      font-weight: inherit !important;
      line-height: inherit !important;
      color: inherit !important;
      text-decoration: none !important;
    }}
    div[style*=""margin: 16px 0;""] {{
      margin: 0 !important;
    }}
    body {{
      width: 100% !important;
      height: 100% !important;
      padding: 0 !important;
      margin: 0 !important;
      background-color: #e9ecef;
    }}
    table {{
      border-collapse: collapse !important;
    }}
    a {{
      color: #1a82e2;
    }}
    img {{
      height: auto;
      line-height: 100%;
      text-decoration: none;
      border: 0;
      outline: none;
    }}
    .form-container {{
      padding: 24px;
      background: #fff;
      border-radius: 6px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.03);
      font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif;
      max-width: 400px;
      margin: 24px auto 0 auto;
    }}
    .form-container label {{
      font-size: 16px;
      display: block;
      margin-bottom: 8px;
    }}
    .form-container input[type=""email""] {{
      width: 100%;
      padding: 10px;
      font-size: 16px;
      margin-bottom: 16px;
      border: 1px solid #d4dadf;
      border-radius: 4px;
      box-sizing: border-box;
    }}
    .form-container button {{
      background-color: #1a82e2;
      color: #fff;
      padding: 12px 24px;
      font-size: 16px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      width: 100%;
    }}
    .form-container button:hover {{
      background-color: #155fa0;
    }}
  </style>
</head>
<body>
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href=""https://zorlen.com"" target=""_blank"" style=""display: inline-block;"">
                <img src=""https://zorlen.com/favicon.ico"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
              <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px; color: red;"">Email verification failed!</h1>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#ffffff"" style=""padding: 24px;"">
              <p style=""margin: 0; font-size: 16px; line-height: 24px;"">Enter your email below to resend the verification email.</p>
              <div class=""form-container"">
                <form method=""get"" action=""{safeLink}"">
                  <label for=""email"">Email address</label>
                  <input type=""email"" id=""email"" name=""email"" required placeholder=""you@example.com"">
                  <button type=""submit"">Resend Verification Email</button>
                </form>
              </div>
            </td>
          </tr>
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Cheers,<br> The RepetiGo Team</p>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">You received this email because we received a request for email verification for your account. If you didn't request email verification you can safely ignore this message.</p>
              <p style=""margin: 0;"">Ho Chi Minh City, Vietnam</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";
        }

        public static string NotifyResendVerificationEmailHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Confirmation</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
    @media screen {
      @font-face {
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 400;
        src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
      }
      @font-face {
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 700;
        src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
      }
    }
    body, table, td, a {
      -ms-text-size-adjust: 100%;
      -webkit-text-size-adjust: 100%;
    }
    table, td {
      mso-table-rspace: 0pt;
      mso-table-lspace: 0pt;
    }
    img {
      -ms-interpolation-mode: bicubic;
    }
    a[x-apple-data-detectors] {
      font-family: inherit !important;
      font-size: inherit !important;
      font-weight: inherit !important;
      line-height: inherit !important;
      color: inherit !important;
      text-decoration: none !important;
    }
    div[style*=""margin: 16px 0;""] {
      margin: 0 !important;
    }
    body {
      width: 100% !important;
      height: 100% !important;
      padding: 0 !important;
      margin: 0 !important;
      background-color: #e9ecef;
    }
    table {
      border-collapse: collapse !important;
    }
    a {
      color: #1a82e2;
    }
    img {
      height: auto;
      line-height: 100%;
      text-decoration: none;
      border: 0;
      outline: none;
    }
    .success-message {
      color: #28a745;
      font-size: 32px;
      font-weight: 700;
      letter-spacing: -1px;
      line-height: 48px;
      margin: 0;
    }
    .info-message {
      font-size: 16px;
      line-height: 24px;
      color: #333;
      margin: 0 0 16px 0;
    }
    .form-container {
      padding: 24px;
      background: #fff;
      border-radius: 6px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.03);
      font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif;
      max-width: 400px;
      margin: 24px auto 0 auto;
    }
  </style>
</head>
<body>
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href=""https://zorlen.com"" target=""_blank"" style=""display: inline-block;"">
                <img src=""https://zorlen.com/favicon.ico"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
              <h1 class=""success-message"">Verification Email Resent!</h1>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#ffffff"" style=""padding: 24px;"">
              <p class=""info-message"">
                Your verification email has been successfully resent.<br>
                Please check your inbox (and spam folder) for the new verification email.<br>
                If you do not receive it within a few minutes, please try again or contact support.
              </p>
            </td>
          </tr>
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Cheers,<br> The RepetiGo Team</p>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">You received this email because you requested to resend the verification email for your account. If you didn't make this request, you can safely ignore this message.</p>
              <p style=""margin: 0;"">Ho Chi Minh City, Vietnam</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";
        }

        public static string NotifyNotFoundHtml(string safeLink)
        {
            return $@"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Not Found</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
    @media screen {{
      @font-face {{
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 400;
        src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
      }}
      @font-face {{
        font-family: 'Source Sans Pro';
        font-style: normal;
        font-weight: 700;
        src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
      }}
    }}
    body, table, td, a {{
      -ms-text-size-adjust: 100%;
      -webkit-text-size-adjust: 100%;
    }}
    table, td {{
      mso-table-rspace: 0pt;
      mso-table-lspace: 0pt;
    }}
    img {{
      -ms-interpolation-mode: bicubic;
    }}
    a[x-apple-data-detectors] {{
      font-family: inherit !important;
      font-size: inherit !important;
      font-weight: inherit !important;
      line-height: inherit !important;
      color: inherit !important;
      text-decoration: none !important;
    }}
    div[style*=""margin: 16px 0;""] {{
      margin: 0 !important;
    }}
    body {{
      width: 100% !important;
      height: 100% !important;
      padding: 0 !important;
      margin: 0 !important;
      background-color: #e9ecef;
    }}
    table {{
      border-collapse: collapse !important;
    }}
    a {{
      color: #1a82e2;
    }}
    img {{
      height: auto;
      line-height: 100%;
      text-decoration: none;
      border: 0;
      outline: none;
    }}
    .form-container {{
      padding: 24px;
      background: #fff;
      border-radius: 6px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.03);
      font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif;
      max-width: 400px;
      margin: 24px auto 0 auto;
    }}
    .form-container label {{
      font-size: 16px;
      display: block;
      margin-bottom: 8px;
    }}
    .form-container input[type=""email""] {{
      width: 100%;
      padding: 10px;
      font-size: 16px;
      margin-bottom: 16px;
      border: 1px solid #d4dadf;
      border-radius: 4px;
      box-sizing: border-box;
    }}
    .form-container button {{
      background-color: #1a82e2;
      color: #fff;
      padding: 12px 24px;
      font-size: 16px;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      width: 100%;
    }}
    .form-container button:hover {{
      background-color: #155fa0;
    }}
    .alert {{
      background-color: #fff3cd;
      color: #856404;
      border: 1px solid #ffeeba;
      border-radius: 4px;
      padding: 12px 16px;
      margin-bottom: 16px;
      font-size: 16px;
      font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif;
      text-align: left;
      max-width: 400px;
      margin-left: auto;
      margin-right: auto;
    }}
  </style>
</head>
<body>
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href=""https://zorlen.com"" target=""_blank"" style=""display: inline-block;"">
                <img src=""https://zorlen.com/favicon.ico"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf;"">
              <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px; color: #d9534f;"">Email address not found</h1>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#ffffff"" style=""padding: 24px;"">
              <div class=""alert"">
                <strong>Alert:</strong> The email address you entered was not found in our records. Please check and try again or use a different email.
              </div>
              <p style=""margin: 0; font-size: 16px; line-height: 24px;"">Enter your email below to resend the verification email.</p>
              <div class=""form-container"">
                <form method=""get"" action=""{safeLink}"">
                  <label for=""email"">Email address</label>
                  <input type=""email"" id=""email"" name=""email"" required placeholder=""you@example.com"">
                  <button type=""submit"">Resend Verification Email</button>
                </form>
              </div>
            </td>
          </tr>
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Cheers,<br> The RepetiGo Team</p>
            </td>
          </tr>
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">You received this email because we received a request for email verification for your account. If you didn't request email verification you can safely ignore this message.</p>
              <p style=""margin: 0;"">Ho Chi Minh City, Vietnam</p>
            </td>
          </tr>
        </table>
      </td>
    </tr>
  </table>
</body>
</html>
";
        }

        public static string GetEmailPasswordResetVerificationHtml(string code)
        {
            return $@"<!DOCTYPE html>
<html>
<head>

  <meta charset=""utf-8"">
  <meta http-equiv=""x-ua-compatible"" content=""ie=edge"">
  <title>Email Confirmation</title>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <style type=""text/css"">
  /**
   * Google webfonts. Recommended to include the .woff version for cross-client compatibility.
   */
  @media screen {{
    @font-face {{
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 400;
      src: local('Source Sans Pro Regular'), local('SourceSansPro-Regular'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/ODelI1aHBYDBqgeIAH2zlBM0YzuT7MdOe03otPbuUS0.woff) format('woff');
    }}
    @font-face {{
      font-family: 'Source Sans Pro';
      font-style: normal;
      font-weight: 700;
      src: local('Source Sans Pro Bold'), local('SourceSansPro-Bold'), url(https://fonts.gstatic.com/s/sourcesanspro/v10/toadOcfmlt9b38dHJxOBGFkQc6VGVFSmCnC_l7QZG60.woff) format('woff');
    }}
  }}
  /**
   * Avoid browser level font resizing.
   * 1. Windows Mobile
   * 2. iOS / OSX
   */
  body,
  table,
  td,
  a {{
    -ms-text-size-adjust: 100%; /* 1 */
    -webkit-text-size-adjust: 100%; /* 2 */
  }}
  /**
   * Remove extra space added to tables and cells in Outlook.
   */
  table,
  td {{
    mso-table-rspace: 0pt;
    mso-table-lspace: 0pt;
  }}
  /**
   * Better fluid images in Internet Explorer.
   */
  img {{
    -ms-interpolation-mode: bicubic;
  }}
  /**
   * Remove blue links for iOS devices.
   */
  a[x-apple-data-detectors] {{
    font-family: inherit !important;
    font-size: inherit !important;
    font-weight: inherit !important;
    line-height: inherit !important;
    color: inherit !important;
    text-decoration: none !important;
  }}
  /**
   * Fix centering issues in Android 4.4.
   */
  div[style*=""margin: 16px 0;""] {{
    margin: 0 !important;
  }}
  body {{
    width: 100% !important;
    height: 100% !important;
    padding: 0 !important;
    margin: 0 !important;
  }}
  /**
   * Collapse table borders to avoid space between cells.
   */
  table {{
    border-collapse: collapse !important;
  }}
  a {{
    color: #1a82e2;
  }}
  img {{
    height: auto;
    line-height: 100%;
    text-decoration: none;
    border: 0;
    outline: none;
  }}
  </style>

</head>
<body style=""background-color: #e9ecef;"">

  <!-- start preheader -->
  <div class=""preheader"" style=""display: none; max-width: 0; max-height: 0; overflow: hidden; font-size: 1px; line-height: 1px; color: #fff; opacity: 0;"">
    A preheader is the short summary text that follows the subject line when an email is viewed in the inbox.
  </div>
  <!-- end preheader -->

  <!-- start body -->
  <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">

    <!-- start logo -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" valign=""top"" style=""padding: 36px 24px;"">
              <a href=""https://zorlen.com"" target=""_blank"" style=""display: inline-block;"">
                <img src=""https://zorlen.com/favicon.ico"" alt=""Logo"" border=""0"" width=""48"" style=""display: block; width: 48px; max-width: 48px; min-width: 48px;"">
              </a>
            </td>
          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end logo -->

    <!-- start hero -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">
          <tr>
            <td align=""center"" bgcolor=""#ffffff"" style=""padding: 36px 24px 0; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; border-top: 3px solid #d4dadf; text-align: center;"">
  <h1 style=""margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px;"">Password reset code</h1>
</td>

          </tr>
        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end hero -->

    <!-- start copy block -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">
              <p style=""margin: 0;"">Use the code below to confirm your email address. If you didn't request password resest for <b>RepetiGo</b> account, you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start copy -->
          <tr>
            <td align=""right"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 32px; line-height: 24px;"">
              <p style=""margin: 0; text-align: center""><b>{code}</b></p>
            </td>
          </tr>
          <!-- end copy -->

          <!-- start copy -->
          <tr>
            <td align=""left"" bgcolor=""#ffffff"" style=""padding: 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-bottom: 3px solid #d4dadf"">
              <p style=""margin: 0;"">Cheers,<br> The RepetiGo Team</p>
            </td>
          </tr>
          <!-- end copy -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end copy block -->

    <!-- start footer -->
    <tr>
      <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 24px;"">
        <!--[if (gte mso 9)|(IE)]>
        <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"">
        <tr>
        <td align=""center"" valign=""top"" width=""600"">
        <![endif]-->
        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px;"">

          <!-- start permission -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">You received this email because we received a request for password reset for your account. If you didn't request password reset you can safely delete this email.</p>
            </td>
          </tr>
          <!-- end permission -->

          <!-- start unsubscribe -->
          <tr>
            <td align=""center"" bgcolor=""#e9ecef"" style=""padding: 12px 24px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 14px; line-height: 20px; color: #666;"">
              <p style=""margin: 0;"">Ho Chi Minh City, Vietnam</p>
            </td>
          </tr>
          <!-- end unsubscribe -->

        </table>
        <!--[if (gte mso 9)|(IE)]>
        </td>
        </tr>
        </table>
        <![endif]-->
      </td>
    </tr>
    <!-- end footer -->

  </table>
  <!-- end body -->

</body>
</html>";
        }

        public static string GetPromptTemplate(string topic, string frontText, string backText)
        {
            // If the user provided a starting point, note it. Otherwise, create from scratch.
            string inputContext = string.IsNullOrWhiteSpace(frontText) && string.IsNullOrWhiteSpace(backText)
                ? $"The user wants a flashcard for the topic: \"{topic}\"."
                : $"The user provided a starting point for the topic \"{topic}\" and wants it improved.\n- Provided Front: \"{frontText}\"\n- Provided Back: \"{backText}\"";

            return $@"You are a flashcard assistant that helps users learn complex topics quickly.
Your tone must be **casual and straightforward**, like a smart friend explaining something.
Your goal is to make the card **easy to understand and memorize**.

{inputContext}

**Instructions:**
1.  Rewrite or generate a flashcard with a 'FrontText' and a 'BackText'.
2.  Use **simple language**. Avoid formal jargon and overly academic words.
3.  If the topic is complex, use a **simple analogy or metaphor** in the BackText.
4.  The FrontText should be a direct question. The BackText should be the clear answer.
5.  Do not include any special characters or formatting in the text.

Respond ONLY with a single, raw JSON object in the following format. Do not include ```json``` or any other markdown.

{{
  ""FrontText"": ""..."",
  ""BackText"": ""...""
}}";
        }

        public static string GetVisualIdeaPrompt(string frontText, string backText)
        {
            return $@"
You are a creative assistant. Based on the following flashcard content, describe a simple, clean, and iconic image that would help a student remember this concept.

**Flashcard Front:** ""{frontText}""
**Flashcard Back:** ""{backText}""

Describe only the visual elements of the image. Do not explain why. Just describe what to draw. Be literal.

**Example:** If the card is about an API, a good description would be: ""A simple diagram of a restaurant scene where a waiter takes an order from a customer to the kitchen.""

Your description:";
        }
    }
}
