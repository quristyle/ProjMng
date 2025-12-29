import { defineAsyncComponent } from "vue";

export interface MenuItem {
  icon?: any;
  name: string;
  path?: string;
  subItems?: MenuItem[];
  pro?: boolean;
  new?: boolean;
}

export interface MenuGroup {
  title: string;
  items: MenuItem[];
}

export const menuGroups: MenuGroup[] = [
  {
    title: "Menu",
    items: [
      {
        icon: defineAsyncComponent(() => import("../../icons").then((m) => m.GridIcon)),
        name: "Dashboard",
        subItems: [{ name: "Ecommerce", path: "/", pro: false }],
      },
      {
        icon: defineAsyncComponent(() => import("../../icons").then((m) => m.CalenderIcon)),
        name: "Calendar",
        path: "/calendar",
      },
      {
        icon: defineAsyncComponent(() => import("../../icons").then((m) => m.UserCircleIcon)),
        name: "User Profile",
        path: "/profile",
      },

      {
        name: "Forms",
        icon: defineAsyncComponent(() => import("../../icons").then((m) => m.ListIcon)),
        subItems: [
          { name: "Form Elements", path: "/form-elements", pro: false },
        ],
      },
      {
        name: "Tables",
        icon: defineAsyncComponent(() => import("../../icons").then((m) => m.TableIcon)),
        subItems: [{ name: "Basic Tables", path: "/basic-tables", pro: false }],
      },
      {
        name: "Pages",
        icon: defineAsyncComponent(() => import("../../icons").then((m) => m.PageIcon)),
        subItems: [
          { name: "Black Page", path: "/blank", pro: false },
          { name: "404 Page", path: "/error-404", pro: false },
        ],
      },
    ],
  },
  {
    title: "Others",
    items: [
      {
        icon: defineAsyncComponent(() => import("../../icons").then((m) => m.PieChartIcon)),
        name: "Charts",
        subItems: [
          { name: "Line Chart", path: "/line-chart", pro: false },
          { name: "Bar Chart", path: "/bar-chart", pro: false },
        ],
      },
      {
        icon: defineAsyncComponent(() => import("@/icons/BoxCubeIcon.vue")),
        name: "Ui Elements",
        subItems: [
          { name: "Alerts", path: "/alerts", pro: false },
          { name: "Avatars", path: "/avatars", pro: false },
          { name: "Badge", path: "/badge", pro: false },
          { name: "Buttons", path: "/buttons", pro: false },
          { name: "Images", path: "/images", pro: false },
          { name: "Videos", path: "/videos", pro: false },
        ],
      },
      {
        icon: defineAsyncComponent(() => import("../../icons").then((m) => m.PlugInIcon)),
        name: "Authentication",
        subItems: [
          { name: "Signin", path: "/signin", pro: false },
          { name: "Signup", path: "/signup", pro: false },
        ],
      },
      // ... Add other menu items here
    ],
  },
];
