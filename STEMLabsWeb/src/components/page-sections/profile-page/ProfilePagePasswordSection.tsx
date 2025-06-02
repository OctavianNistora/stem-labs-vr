import { type ChangeEvent, useContext, useState } from "react";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import axios from "axios";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import { Button, CircularProgress, Stack, TextField } from "@mui/material";

export default function ProfilePagePasswordSection() {
  const [newPassword, setNewPassword] = useState("");
  const [newPasswordConfirmation, setNewPasswordConfirmation] = useState("");
  const [currentPassword, setCurrentPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  function handleNewPasswordChange(event: ChangeEvent<HTMLInputElement>) {
    setNewPassword(event.target.value);
  }

  function handleConfirmNewPasswordChange(
    event: ChangeEvent<HTMLInputElement>,
  ) {
    setNewPasswordConfirmation(event.target.value);
  }

  function handleCurrentPasswordChange(event: ChangeEvent<HTMLInputElement>) {
    setCurrentPassword(event.target.value);
  }

  function handleSubmit() {
    const data = {
      newPassword: newPassword,
      currentPassword: currentPassword,
    };

    setIsSubmitting(true);
    axios
      .put(
        `${import.meta.env.VITE_API_URL}/api/users/${user?.uid}/password`,
        data,
        {
          headers: {
            Authorization: `Bearer ${user?.accessToken}`,
          },
        },
      )
      .then(() => {
        addToast({
          message: "Password updated successfully",
          variant: "success",
        });

        setNewPassword("");
        setNewPasswordConfirmation("");
        setCurrentPassword("");
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
    <Stack gap={4}>
      <TextField
        type="password"
        label="New Password"
        value={newPassword}
        onChange={handleNewPasswordChange}
      />
      <TextField
        type="password"
        label="Confirm New Password"
        error={!doPasswordsMatch}
        helperText={!doPasswordsMatch ? "Passwords do not match" : undefined}
        value={newPasswordConfirmation}
        onChange={handleConfirmNewPasswordChange}
      />
      <TextField
        type="password"
        label="Current Password"
        value={currentPassword}
        onChange={handleCurrentPasswordChange}
      />
      <Button
        disabled={
          !(
            !!newPassword &&
            !!newPasswordConfirmation &&
            !!currentPassword &&
            doPasswordsMatch
          ) || isSubmitting
        }
        variant="contained"
        color="primary"
        onClick={handleSubmit}
      >
        {isSubmitting ? <CircularProgress /> : "Update Password"}
      </Button>
    </Stack>
  );
}
