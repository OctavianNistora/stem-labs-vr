import { type ChangeEvent, useContext, useState } from "react";
import { useNavigate } from "react-router";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import {
  Box,
  Button,
  CircularProgress,
  TextField,
  Typography,
} from "@mui/material";
import { axiosRequestWithAutoReauth } from "../../../helpers/axiosRequestWithAutoReauth.tsx";

type ResetPasswordSectionProps = {
  username: string;
};

export default function ResetPasswordSection({
  username,
}: ResetPasswordSectionProps) {
  const [newPassword, setNewPassword] = useState("");
  const [newPasswordConfirmation, setNewPasswordConfirmation] = useState("");
  const [token, setToken] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigate = useNavigate();
  const { addToast } = useContext(ToastContext);
  const { setUser } = useContext(AuthContext);

  function handlePasswordChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) {
    setNewPassword(event.target.value);
  }

  function handlePasswordConfirmationChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) {
    setNewPasswordConfirmation(event.target.value);
  }

  function handleTokenChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) {
    setToken(event.target.value);
  }

  function handleSubmit() {
    const data = {
      username: username,
      newPassword: newPassword,
      token: token,
    };

    setIsSubmitting(true);
    axiosRequestWithAutoReauth(
      {
        method: "POST",
        url: `${import.meta.env.VITE_API_URL}/api/recovery/password-reset`,
        data: data,
        headers: { "Content-Type": "application/json" },
      },
      setUser,
    )
      .then(() => {
        addToast({
          message: "Password reset successfully.",
          variant: "success",
        });
        navigate("/login");
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
      })
      .finally(() => {
        setIsSubmitting(false);
      });
  }

  const doPasswordsMatch =
    !newPassword ||
    !newPasswordConfirmation ||
    newPassword == newPasswordConfirmation;

  return (
    <Box display="flex" flexDirection="column" gap={2} padding={2}>
      <Typography variant="h4" align="center" marginBottom={5}>
        Reset your password.
      </Typography>
      <TextField
        label={"New Password"}
        type="password"
        value={newPassword}
        onChange={handlePasswordChange}
      />
      <TextField
        label={"Confirm New Password"}
        type="password"
        error={!doPasswordsMatch}
        helperText={!doPasswordsMatch ? "Passwords do not match." : undefined}
        value={newPasswordConfirmation}
        onChange={handlePasswordConfirmationChange}
      />
      <TextField
        label={"Recovery Code"}
        value={token}
        onChange={handleTokenChange}
      />
      <Button
        disabled={isSubmitting}
        variant="contained"
        size="large"
        onClick={handleSubmit}
      >
        {isSubmitting ? <CircularProgress /> : "Reset Password"}
      </Button>
    </Box>
  );
}
