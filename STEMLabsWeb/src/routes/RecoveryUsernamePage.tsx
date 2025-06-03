import {
  Box,
  Button,
  CircularProgress,
  TextField,
  Typography,
} from "@mui/material";
import { type ChangeEvent, useContext, useState } from "react";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { useNavigate } from "react-router";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";

export default function RecoveryUsernamePage() {
  const [email, setEmail] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigate = useNavigate();
  const { addToast } = useContext(ToastContext);
  const { setUser } = useContext(AuthContext);

  function handleEmailChange(
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ) {
    setEmail(event.target.value);
  }

  function handleSubmit() {
    setIsSubmitting(true);

    axiosRequestWithAutoReauth(
      {
        method: "post",
        url: `${import.meta.env.VITE_API_URL}/api/recovery/username-reminder`,
        data: email,
        headers: {
          "Content-Type": "application/json",
        },
      },
      setUser,
    )
      .then(() => {
        addToast({
          message: "A recovery email has been sent to your email address.",
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

  return (
    <Box display="flex" flexDirection="column" gap={2} padding={2}>
      <Typography variant="h4" align="center" marginBottom={5}>
        Enter your email address to recover your username.
      </Typography>
      <TextField label={"Email"} value={email} onChange={handleEmailChange} />
      <Button
        disabled={isSubmitting}
        variant="contained"
        size="large"
        onClick={handleSubmit}
      >
        {isSubmitting ? <CircularProgress /> : "Send Recovery Email"}
      </Button>
    </Box>
  );
}
