import {
  Box,
  Button,
  CircularProgress,
  TextField,
  Typography,
} from "@mui/material";
import { useContext, useState, type MouseEvent } from "react";
import { AuthContext } from "../contexts/AuthContext.tsx";
import { Link, useNavigate } from "react-router";
import { ToastContext } from "../layouts/ToastLayout.tsx";
import { toastErrorMessageHandle } from "../helpers/ToastErrorMessageHandle.tsx";
import { axiosRequestWithAutoReauth } from "../helpers/axiosRequestWithAutoReauth.tsx";

export default function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isAuthenticating, setIsAuthenticating] = useState(false);
  const { addToast } = useContext(ToastContext);
  const { setUser } = useContext(AuthContext);
  const navigate = useNavigate();

  function handleSubmit(_: MouseEvent<HTMLButtonElement> | undefined) {
    const data = {
      username: username,
      password: password,
      respondWithRefreshToken: true,
    };
    setIsAuthenticating(true);
    axiosRequestWithAutoReauth(
      {
        method: "POST",
        url: `${import.meta.env.VITE_API_URL}/api/auth/session`,
        data: data,
      },
      setUser,
    )
      .then((res) => {
        const { uid, accessToken, refreshToken, role } = res.data;
        localStorage.setItem("refreshToken", refreshToken);
        setUser({ uid, accessToken, role });
        navigate("/");
      })
      .catch((error) => {
        toastErrorMessageHandle(addToast, setUser, error);
      })
      .finally(() => {
        setIsAuthenticating(false);
      });
  }

  return (
    <>
      <Box padding={2} justifyItems="center">
        <Typography variant="h3">STEM Labs VR</Typography>
        <Typography variant="h5">Authentication</Typography>
      </Box>
      <Box display="flex" width="80%" flexDirection="column" gap={2}>
        <TextField
          id="username-field"
          fullWidth
          type="text"
          autoComplete="username"
          label="Username"
          value={username}
          onChange={(e) => setUsername(e.currentTarget.value)}
        />
        <TextField
          id="password-field"
          fullWidth
          type="password"
          autoComplete="current-password"
          label="Password"
          value={password}
          onChange={(e) => setPassword(e.currentTarget.value)}
        />
        <Button
          id="login-button"
          disabled={isAuthenticating}
          fullWidth
          size="large"
          variant="contained"
          onClick={handleSubmit}
        >
          {isAuthenticating ? <CircularProgress /> : "Log in"}
        </Button>
        <Button
          id="recovery-button"
          disabled={isAuthenticating}
          fullWidth
          size="small"
          component={Link}
          to="/recovery"
        >
          Cannot log in
        </Button>
      </Box>
    </>
  );
}
