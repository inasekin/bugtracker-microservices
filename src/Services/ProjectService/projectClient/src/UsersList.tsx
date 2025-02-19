import * as React from "react" 
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { UserRolesRecord, columns } from "./projectTables/userColumns"
import { DataTable } from "./projectTables/data-table"
 
//async function getData(): Promise<UserRolesRecord[]> {
function getData(): UserRolesRecord[] {
  return [
    {
      id: "1",
      user: "Вадим С.",
      roles: "Разработчик, Тестировщик, Руководитель проекта",
      command: "Редактировать",
    },
    {
      id: "2",
      user: "Иван Н.",
      roles: "Разработчик, Тестировщик, DevOps",
      command: "Редактировать",
    },
    {
      id: "3",
      user: "Артём П.",
      roles: "Разработчик, Тестировщик",
      command: "Редактировать",
    },
    {
      id: "4",
      user: "Павел С.",
      roles: "Разработчик, Тестировщик",
      command: "Редактировать",
    },
    {
      id: "5",
      user: "Алексей М.",
      roles: "Разработчик, Тестировщик",
      command: "Редактировать",
    },
    {
      id: "6",
      user: "Александр Н.",
      roles: "Покинул проект",
      command: "Редактировать",
    }
  ];
}
 
export function UsersList() {


  const data = getData();

  return (
    <Card>
      <CardHeader>
        <CardTitle>Участники проекта</CardTitle>
        <CardDescription className="my-2">Для проекта необходимо указать участников и задать роли. <br />Исходя из указанных ролей будут назначены заранее определенные права доступа.</CardDescription>
      </CardHeader>
      <CardContent>
 
    	<div className="container mx-auto ">
      		<DataTable columns={columns} data={data} /> 
	    </div>
      </CardContent>
      <CardFooter className="flex justify-between">
        <Button id="addUser">Добавить участника...</Button>
      </CardFooter>
    </Card>
  )
}