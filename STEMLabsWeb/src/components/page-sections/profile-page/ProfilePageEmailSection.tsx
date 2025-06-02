import { type ChangeEvent, useContext, useEffect, useState } from "react";
import axios from "axios";
import { Button, CircularProgress, Stack, TextField } from "@mui/material";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import { ProfileStyledTextField } from "../../ProfileStyledMUIComponents.tsx";
import isEmail from "validator/lib/isEmail";

export default function ProfilePageEmailSection() {
  const [currentEmail, setCurrentEmail] = useState(" ");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { addToast } = useContext(ToastContext);
  const { user, setUser } = useContext(AuthContext);

  useEffect(() => {
    if (!user) {
      return;
    }

    axios
      .get<string>(
        `${import.meta.env.VITE_API_URL}/api/users/${user.uid}/email`,
        {
          headers: {
            Authorization: `Bearer ${user.accessToken}`,
          },
        },
      )
      .then((response) => {
        setCurrentEmail(response.data);
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
      });
  }, []);

  function handleEmailChange(event: ChangeEvent<HTMLInputElement>) {
    setEmail(event.target.value);
  }

  function handlePasswordChange(event: ChangeEvent<HTMLInputElement>) {
    setPassword(event.target.value);
  }

  function handleSubmit() {
    const data = {
      newEmail: email,
      password: password,
    };

    setIsSubmitting(true);
    axios
      .put(
        `${import.meta.env.VITE_API_URL}/api/users/${user?.uid}/email`,
        data,
        {
          headers: {
            Authorization: `Bearer ${user?.accessToken}`,
          },
        },
      )
      .then(() => {
        addToast({
          message: "Email updated successfully",
          variant: "success",
        });
        setCurrentEmail(email);
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
      })
      .finally(() => {
        setEmail("");
        setPassword("");
        setIsSubmitting(false);
      });
  }

  const isEmailValid = !email || isEmail(email);

  return (
    <Stack gap={4}>
      <ProfileStyledTextField
        readOnly
        label="Current Email Address"
        value={currentEmail}
      />
      <TextField
        type="email"
        label="New Email Address"
        value={email}
        error={!isEmailValid}
        helperText={!isEmailValid ? "Invalid email address" : undefined}
        onChange={handleEmailChange}
      />
      <TextField
        type="password"
        autoComplete="new-password"
        label="Current Password"
        value={password}
        onChange={handlePasswordChange}
      />
      <Button
        disabled={!(!!email && !!password && isEmailValid) || isSubmitting}
        variant="contained"
        color="primary"
        onClick={handleSubmit}
      >
        {isSubmitting ? <CircularProgress /> : "Update Email"}
      </Button>
    </Stack>
  );
}
