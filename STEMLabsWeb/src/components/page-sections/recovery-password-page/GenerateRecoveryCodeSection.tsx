import { type ChangeEvent, useContext, useState } from "react";
import { ToastContext } from "../../../layouts/ToastLayout.tsx";
import { AuthContext } from "../../../contexts/AuthContext.tsx";
import axios from "axios";
import { toastErrorMessageHandle } from "../../../helpers/ToastErrorMessageHandle.tsx";
import {
  Box,
  Button,
  CircularProgress,
  TextField,
  Typography,
} from "@mui/material";

type GenerateRecoveryCodeSectionProps = {
  setUsernameParent: (username: string) => void;
};

export default function GenerateRecoveryCodeSection({
  setUsernameParent,
}: GenerateRecoveryCodeSectionProps) {
  const [username, setUsername] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const { addToast } = useContext(ToastContext);
  const { setUser } = useContext(AuthContext);

  function handleUsernameChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) {
    setUsername(event.target.value);
  }

  function handleSubmit() {
    setIsSubmitting(true);

    axios
      .post(
        `${import.meta.env.VITE_API_URL}/api/recovery/password-request`,
        username,
        { headers: { "Content-Type": "application/json" } },
      )
      .then(() => {
        setUsernameParent(username);
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
      })
      .finally(() => {
        setIsSubmitting(false);
      });
  }

  return (
    <Box display="flex" flexDirection="column" gap={2} padding={2}>
      <Typography variant="h4" align="center" marginBottom={5}>
        Enter your username to recover your password.
      </Typography>
      <TextField
        label={"Username"}
        value={username}
        onChange={handleUsernameChange}
      />
      <Button
        disabled={isSubmitting}
        variant="contained"
        size="large"
        onClick={handleSubmit}
      >
        {isSubmitting ? <CircularProgress /> : "Next"}
      </Button>
    </Box>
  );
}
