"use client"
 
import { ColumnDef } from "@tanstack/react-table"
import { Button } from "@/components/ui/button"
 
// This type is used to define the shape of our data.
// You can use a Zod schema here if you want.
export type UserRolesRecord = {
  id: string
  user: string
  roles: string
  command: string
}
 
export const columns: ColumnDef<UserRolesRecord>[] = [
  {
    accessorKey: "user",
    header: () => <div className="text-left">Пользователь/Группа</div>,
    cell: ({ row }) => { 
      const userRoles = row.original;
      const formatted = userRoles.user; 
      return <div className="text-left font-medium">{formatted}</div>
    }
  },
  {
    accessorKey: "roles",
    header: "Роли",
  },
  {
    accessorKey: "command",
    header: "",
    cell: ({ row }) => { 
      const userRoles = row.original;
      const command = userRoles.command;
      return (<div className="flex justify-end">
	      <Button variant="secondary">{command}</Button>
        <Button variant="destructive">Удалить</Button>
      </div>);
    }
  },
];