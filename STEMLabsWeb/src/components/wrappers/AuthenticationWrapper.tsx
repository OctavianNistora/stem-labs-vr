import { useContext, useEffect } from "react";
import { AuthContext } from "../../contexts/AuthContext.tsx";
import { Outlet, useNavigate } from "react-router";
import { CircularProgress } from "@mui/material";

type AuthenticationWrapperProps = {
  reverse?: boolean;
};

export default function AuthenticationWrapper(
  props: AuthenticationWrapperProps,
) {
  const { reverse } = props;
  const { user, isAuthInitialized } = useContext(AuthContext);
  const navigate = useNavigate();

  useEffect(() => {
    if (isAuthInitialized) {
      if (reverse && user) {
        navigate("/");
      } else if (!reverse && !user) {
        navigate("/login");
      }
    }
  }, [reverse, user, isAuthInitialized]);

  return <>{isAuthInitialized ? <Outlet /> : <CircularProgress />}</>;
}
