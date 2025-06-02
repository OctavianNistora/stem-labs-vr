import { Button } from "@mui/material";
import { Link } from "react-router";

export default function RecoverySelectionPage() {
  return (
    <>
      <Button variant="contained" size="large" component={Link} to="username">
        Forgot Username
      </Button>
      <Button variant="contained" size="large" component={Link} to="password">
        Forgot Password
      </Button>
    </>
  );
}
