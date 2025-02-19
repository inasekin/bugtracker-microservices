"use client"
 
import { ColumnDef } from "@tanstack/react-table"
import { Button } from "@/components/ui/button"
 
// This type is used to define the shape of our data.
// You can use a Zod schema here if you want.
export type VersionRecord = {
  id: string,
  version: string,
  description: string
}
 
export const columns: ColumnDef<VersionRecord>[] = [
  {
    accessorKey: "version",
    header: () => <div className="text-left">Версия</div>,
    cell: ({ row }) => { 
      const formatted = row.original.version; 
      return <div className="text-left font-medium">{formatted}</div>
    }
  },
  {
    accessorKey: "description",
    header: () => <div className="text-left">Описание</div>,
  },
  {
    accessorKey: "command",
    header: "",
    cell: ({ row }) => { 
      const command = row.original.command;
      return (<div className="flex justify-end">
	      <Button variant="secondary">{command}</Button>
	      <Button variant="destructive">Удалить</Button>
      </div>);
    }
  },
];