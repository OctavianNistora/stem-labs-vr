import { jsx as _jsx, jsxs as _jsxs } from "react/jsx-runtime";
import { Box, Drawer, List, ListItem, ListItemButton, ListItemIcon, ListItemText, styled, SvgIcon, } from "@mui/material";
import { Link, Outlet } from "react-router";
import LogoSvg from "../icons/SvgLogo";
import { useContext } from "react";
import { AuthContext } from "../contexts/AuthContext.tsx";
import LocationCityIcon from "@mui/icons-material/LocationCity";
import AssignmentTurnedInIcon from "@mui/icons-material/AssignmentTurnedIn";
import PeopleIcon from "@mui/icons-material/People";
import BiotechIcon from "@mui/icons-material/Biotech";
import AccountBoxIcon from "@mui/icons-material/AccountBox";
import LogoutIcon from "@mui/icons-material/Logout";
const beforeDrawerList = [
    { icon: _jsx(LocationCityIcon, {}), text: "Home Page", link: "/" },
];
const studentDrawerList = [
    {
        icon: _jsx(AssignmentTurnedInIcon, {}),
        text: "View Reports",
        link: "reports",
    },
];
const professorDrawerList = studentDrawerList.concat([]);
const adminDrawerList = professorDrawerList.concat([
    { icon: _jsx(BiotechIcon, {}), text: "Manage Laboratories", link: "laboratories" },
    { icon: _jsx(PeopleIcon, {}), text: "Manage Users", link: "manage-users" },
]);
const afterDrawerList = [
    { icon: _jsx(AccountBoxIcon, {}), text: "Profile", link: "profile" },
];
export function getDrawerList(role) {
    let drawerList;
    switch (role?.toLowerCase()) {
        case "admin":
            drawerList = adminDrawerList;
            break;
        case "professor":
            drawerList = professorDrawerList;
            break;
        case "student":
            drawerList = studentDrawerList;
            break;
        default:
            drawerList = [];
    }
    return beforeDrawerList.concat(drawerList, afterDrawerList);
}
const drawerWidth = "300px";
export default function DrawerLayout() {
    const { user, setUser } = useContext(AuthContext);
    function handleLogout() {
        localStorage.removeItem("refreshToken");
        setUser(null);
    }
    return (_jsxs(Box, { display: "flex", width: "100%", height: "100%", children: [_jsxs(Drawer, { variant: "permanent", slotProps: {
                    paper: {
                        sx: {
                            width: drawerWidth,
                            bgcolor: "primary.main",
                            borderTopRightRadius: 8,
                            borderBottomRightRadius: 8,
                        },
                    },
                }, sx: { width: drawerWidth }, children: [_jsx(SvgIcon, { component: LogoSvg, sx: { fontSize: 96, color: "primary.contrastText" }, viewBox: "0 0 1024 1024" }), _jsxs(List, { id: "drawer-list", sx: { fontSize: 12 }, children: [getDrawerList(user?.role).map((item) => (_jsx(ListItem, { disablePadding: true, children: _jsxs(ListItemButton, { id: `${item.text.toLowerCase().replace(" ", "-")}-drawer-button`, component: Link, to: item.link, children: [_jsx(StyledListItemIcon, { children: item.icon }), _jsx(StyledListItemText, { primary: _jsx(Box, { fontWeight: "normal", children: item.text }) })] }) }, item.text))), _jsx(ListItem, { disablePadding: true, children: _jsxs(ListItemButton, { id: "logout-drawer-button", onClick: handleLogout, children: [_jsx(StyledListItemIcon, { sx: { marginLeft: "2px" }, children: _jsx(LogoutIcon, {}) }), _jsx(StyledListItemText, { primary: "Logout" })] }) }, "Logout")] })] }), _jsx(Box, { width: "100%", height: "100%", justifyItems: "center", alignContent: "center", bgcolor: "#F4F4F4", children: _jsx(Outlet, {}) })] }));
}
const StyledListItemIcon = styled(ListItemIcon)(({ theme }) => ({
    "& svg": {
        fontSize: "2.5em",
        color: theme.palette.primary.contrastText,
    },
}));
const StyledListItemText = styled(ListItemText)(({ theme }) => ({
    color: theme.palette.primary.contrastText,
    "& .MuiListItemText-primary": {
        fontSize: "1.4em",
    },
}));
