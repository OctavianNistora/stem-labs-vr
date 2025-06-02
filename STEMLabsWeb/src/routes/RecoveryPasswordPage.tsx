import { useState } from "react";
import ResetPasswordSection from "../components/page-sections/recovery-password-page/ResetPasswordSection.tsx";
import GenerateRecoveryCodeSection from "../components/page-sections/recovery-password-page/GenerateRecoveryCodeSection.tsx";

export default function RecoveryPasswordPage() {
  const [username, setUsername] = useState("");

  return (
    <>
      {username ? (
        <ResetPasswordSection username={username} />
      ) : (
        <GenerateRecoveryCodeSection setUsernameParent={setUsername} />
      )}
    </>
  );
}
