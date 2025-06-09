import {
  Box,
  Drawer,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  styled,
  SvgIcon,
} from "@mui/material";
import { Link, Outlet } from "react-router";
import LogoSvg from "../icons/SvgLogo";
import { type JSX, useContext } from "react";
import { AuthContext } from "../contexts/AuthContext.tsx";
import LocationCityIcon from "@mui/icons-material/LocationCity";
import AssignmentTurnedInIcon from "@mui/icons-material/AssignmentTurnedIn";
import PeopleIcon from "@mui/icons-material/People";
import BiotechIcon from "@mui/icons-material/Biotech";
import AccountBoxIcon from "@mui/icons-material/AccountBox";
import LogoutIcon from "@mui/icons-material/Logout";

export type DrawerListElement = {
  icon: JSX.Element;
  text: string;
  link: string;
};

const beforeDrawerList: DrawerListElement[] = [
  { icon: <LocationCityIcon />, text: "Home Page", link: "/" },
];

const studentDrawerList: DrawerListElement[] = [
  {
    icon: <AssignmentTurnedInIcon />,
    text: "View Reports",
    link: "reports",
  },
];
const professorDrawerList: DrawerListElement[] = studentDrawerList.concat([]);
const adminDrawerList: DrawerListElement[] = professorDrawerList.concat([
  { icon: <BiotechIcon />, text: "Manage Laboratories", link: "laboratories" },
  { icon: <PeopleIcon />, text: "Manage Users", link: "manage-users" },
]);

const afterDrawerList: DrawerListElement[] = [
  { icon: <AccountBoxIcon />, text: "Profile", link: "profile" },
];

export function getDrawerList(role: string | undefined) {
  let drawerList: DrawerListElement[];
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

  return (
    <Box display="flex" width="100%" height="100%">
      <Drawer
        variant="permanent"
        slotProps={{
          paper: {
            sx: {
              width: drawerWidth,
              bgcolor: "primary.main",
              borderTopRightRadius: 8,
              borderBottomRightRadius: 8,
            },
          },
        }}
        sx={{ width: drawerWidth }}
      >
        <SvgIcon
          component={LogoSvg}
          sx={{ fontSize: 96, color: "primary.contrastText" }}
          viewBox="0 0 1024 1024"
        />
        <List id={"drawer-list"} sx={{ fontSize: 12 }}>
          {getDrawerList(user?.role).map((item) => (
            <ListItem key={item.text} disablePadding>
              <ListItemButton
                id={`${item.text.toLowerCase().replace(" ", "-")}-drawer-button`}
                component={Link}
                to={item.link}
              >
                <StyledListItemIcon>{item.icon}</StyledListItemIcon>
                <StyledListItemText
                  primary={<Box fontWeight="normal">{item.text}</Box>}
                />
              </ListItemButton>
            </ListItem>
          ))}
          <ListItem key="Logout" disablePadding>
            <ListItemButton id={"logout-drawer-button"} onClick={handleLogout}>
              <StyledListItemIcon sx={{ marginLeft: "2px" }}>
                <LogoutIcon />
              </StyledListItemIcon>
              <StyledListItemText primary="Logout" />
            </ListItemButton>
          </ListItem>
        </List>
      </Drawer>
      <Box
        width="100%"
        height="100%"
        justifyItems="center"
        alignContent="center"
        bgcolor="#F4F4F4"
      >
        <Outlet />
      </Box>
    </Box>
  );
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
