import { createTheme, type ThemeOptions } from "@mui/material";

export const themeOptions: ThemeOptions = {
  palette: {
    mode: "light",
    primary: {
      main: "#2e3192",
    },
    secondary: {
      main: "#9c27b0",
    },
    background: {
      paper: "#f5f5fa",
    },
  },
};

export const theme = createTheme(themeOptions);
