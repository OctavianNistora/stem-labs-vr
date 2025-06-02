import {
  createContext,
  type Dispatch,
  type SetStateAction,
  useEffect,
  useMemo,
  useState,
} from "react";
import axios from "axios";
import { Outlet } from "react-router";

type User = {
  uid: string;
  accessToken: string;
  role: "student" | "professor" | "admin";
};

type AuthState = {
  user: User | null;
  setUser: Dispatch<SetStateAction<User | null>>;
  isAuthInitialized: boolean;
};

export const AuthContext = createContext<AuthState>({
  user: null,
  setUser: () => null,
  isAuthInitialized: false,
});

export function AuthContextProvider() {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthInitialized, setIsAuthInitialized] = useState(false);

  useEffect(() => {
    const storageRefreshToken = localStorage.getItem("refreshToken");
    if (!storageRefreshToken) {
      setIsAuthInitialized(true);
      return;
    }

    const abortController = new AbortController();
    axios
      .post(
        `${import.meta.env.VITE_API_URL}/api/auth/refresh-token`,
        storageRefreshToken,
        {
          headers: {
            "Content-Type": "application/json",
          },
          signal: abortController.signal,
        },
      )
      .then((res) => {
        const { uid, accessToken, refreshToken, role } = res.data;
        setUser({ uid, accessToken, role });
        localStorage.setItem("refreshToken", refreshToken);
        setIsAuthInitialized(true);
      })
      .catch((_) => {
        console.log("Refresh token expired");
        localStorage.removeItem("refreshToken");
        setIsAuthInitialized(true);
      });

    return () => abortController.abort();
  }, []);

  const authValue = useMemo(() => {
    return {
      user,
      setUser,
      isAuthInitialized,
    };
  }, [user, setUser, isAuthInitialized]);

  return (
    <AuthContext.Provider value={authValue}>
      {isAuthInitialized ? <Outlet /> : null}
    </AuthContext.Provider>
  );
}
