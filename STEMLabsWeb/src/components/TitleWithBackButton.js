import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, IconButton, Typography } from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { Link } from "react-router";
export default function TitleWithBackButton(props) {
    const { to, title } = props;
    return (_jsxs(Box, { width: "100%", display: "flex", children: [_jsx(Box, { width: 0, height: 40, display: "flex", flexDirection: "row-reverse", children: _jsx(IconButton, { component: Link, to: to, children: _jsx(ArrowBackIcon, {}) }) }), _jsx(Box, { width: "100%", position: "relative", children: _jsx(Typography, { variant: "h4", marginY: 2, textAlign: "center", children: title }) })] }));
}
