import { jsx as _jsx, Fragment as _Fragment } from "react/jsx-runtime";
import { useState } from "react";
import ResetPasswordSection from "../components/page-sections/recovery-password-page/ResetPasswordSection.tsx";
import GenerateRecoveryCodeSection from "../components/page-sections/recovery-password-page/GenerateRecoveryCodeSection.tsx";
export default function RecoveryPasswordPage() {
    const [username, setUsername] = useState("");
    return (_jsx(_Fragment, { children: username ? (_jsx(ResetPasswordSection, { username: username })) : (_jsx(GenerateRecoveryCodeSection, { setUsernameParent: setUsername })) }));
}
